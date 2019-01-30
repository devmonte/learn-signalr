(function () {
    document.getElementById("conversationContainer").style.display = "none";

    var userName = "";

    //var token = ""
    function getToken() {
        console.log("sending request for Token!");

        userName = document.getElementById("userName").value
        var password = document.getElementById("userPassword").value;
        var user = { Name: userName, Password: password };

        var xhttp;
        xhttp = new XMLHttpRequest();
        xhttp.onreadystatechange = function () {
            if (this.readyState == 4 && this.status == 200) {
                console.log(xhttp.response);
                document.getElementById("loginContainer").style.display = "none";
                startConnection(xhttp.response);
            }
        };
        xhttp.open("POST", "https://localhost:5001/api/auth", true);
        xhttp.setRequestHeader('Access-Control-Allow-Origin', '*');
        xhttp.setRequestHeader('Content-Type', 'application/json');
        xhttp.send(JSON.stringify(user));
    }

    function startConnection(token) {

        var connection = new signalR.HubConnectionBuilder()
            .withUrl('https://localhost:5001/chatHub', { accessTokenFactory: () => token })
            .build();

        connection.on("ReceiveMessage", function (message) {
            var label = document.createElement("li");
            label.className = "list-group-item";
            label.textContent = "User: " + message.user + " send: " + message.message;
            document.getElementById("messagesList").appendChild(label);
        });
        connection.on("ReceiveNotification", function (message) {
            var label = document.createElement("li");
            label.className = "list-group-item";
            label.textContent = "Notification: " + message
            document.getElementById("messagesList").appendChild(label);
        });

        connection.start()
            .then(function () {
                console.log("connection started!")
                document.getElementById("conversationContainer").style.display = "block";

            })
            .catch(error => {
                console.error(error.message);
            });

        document.getElementById("sendBtn").addEventListener("click", function (event) {
            var message = document.getElementById("userMessage").value
            connection.invoke("SendMessage", { User: userName, Message: message }).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });
    }


    document.getElementById("loginBtn").addEventListener("click", function (event) {
        event.preventDefault();
        getToken();
    });

})()
