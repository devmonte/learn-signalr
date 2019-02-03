(function () {
    document.addEventListener('DOMContentLoaded', function () {

        var userName = "";
        document.getElementById("userName").value = Math.random().toString(36).substring(2, 10);

        function createMessageEntry(encodedName, encodedMsg) {
            var entry = document.createElement('div');
            entry.classList.add("message-entry");
            if (encodedName === userName) {
                entry.innerHTML = `<div class="message-avatar pull-right">${encodedName}</div>` +
                    `<div class="message-content pull-right">${encodedMsg}<div>`;
            } else {
                entry.innerHTML = `<div class="message-avatar pull-left">${encodedName}</div>` +
                    `<div class="message-content pull-left">${encodedMsg}<div>`;
            }
            return entry;
        }

        function bindConnectionMessage(connection) {

            var messageCallback = function (messageDto) {
                if (!messageDto) return;

                var encodedName = messageDto.user;
                var encodedMsg = messageDto.message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                var messageEntry = createMessageEntry(encodedName, encodedMsg);

                var messageBox = document.getElementById('messages');
                messageBox.appendChild(messageEntry);
                messageBox.scrollTop = messageBox.scrollHeight;
            };

            var notificationCallback = function (notification) {
                if (!notification) return;

                var entry = document.createElement('div');
                entry.classList.add("message-entry");
                entry.innerHTML = notification.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
                entry.classList.add("text-center");
                entry.classList.add("system-message");

                var messageBox = document.getElementById('messages');
                messageBox.appendChild(entry);
                messageBox.scrollTop = messageBox.scrollHeight;
            };

            connection.on('receiveMessage', messageCallback);
            connection.on('receiveNotification', notificationCallback);
            connection.onclose(onConnectionError);
        }

        function onConnected(connection) {
            console.log('connection started');
            document.getElementById("conversationContainer").style.display = "block";
            var messageInput = document.getElementById('message');
            messageInput.focus();

            document.getElementById('sendmessage').addEventListener('click', function (event) {
                if (messageInput.value) {
                    connection.send('sendMessage', { User: userName, Message: messageInput.value });
                }

                messageInput.value = '';
                messageInput.focus();
                event.preventDefault();
            });
            document.getElementById('message').addEventListener('keypress', function (event) {
                if (event.keyCode === 13) {
                    event.preventDefault();
                    document.getElementById('sendmessage').click();
                    return false;
                }
            });
        }

        function onConnectionError(error) {
            if (error && error.message) {
                console.error(error.message);
            }
            var modal = document.getElementById('myModal');
            modal.classList.add('in');
            modal.style = 'display: block;';
        }

        function startConnection(token) {

            var connection = new signalR.HubConnectionBuilder()
                .withUrl('https://localhost:5001/chatHub', { accessTokenFactory: () => token })
                .build();

            bindConnectionMessage(connection);
            connection.start()
                .then(function () {
                    onConnected(connection);
                })
                .catch(function (error) {
                    console.error(error.message);
                });
        }

        function getToken() {
            console.log("sending request for Token!");

            userName = document.getElementById("userName").value;
            var password = document.getElementById("userPassword").value;
            var user = { Name: userName, Password: password };

            var xhttp;
            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState === 4 && this.status === 200) {
                    console.log(xhttp.response);
                    document.getElementById("loginContainer").style.display = "none";

                    startConnection(xhttp.response);

                }
                else if (this.readyState === 4 && this.status === 403) {

                    var warning = document.createElement('label');
                    warning.classList.add("label-warning");
                    warning.innerHTML = "Wrong password!";
                    document.getElementById("passwordForm").appendChild(warning);

                }
            };
            xhttp.onerror = function () {

                var warning = document.createElement('label');
                warning.classList.add("warning");
                warning.value = "Upss error!";
                document.getElementById("passwordForm").appendChild(warning);
            };
            xhttp.open("POST", "https://localhost:5001/api/auth", true);
            xhttp.setRequestHeader('Access-Control-Allow-Origin', '*');
            xhttp.setRequestHeader('Content-Type', 'application/json');
            xhttp.send(JSON.stringify(user));
        }

        document.getElementById("loginBtn").addEventListener("click", function (event) {
            event.preventDefault();
            getToken();
        });

    });

})();