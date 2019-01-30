(function () {
    var userName = "";
    document.getElementById("conversationContainer").style.display = "none";

    function startConnection() {

        var connection = new signalR.HubConnectionBuilder()
            .withUrl('chatHub')
            .build();

        connection.on("ReceiveMessage", function (user, message) {
            var label = document.createElement("li");
            label.className = "list-group-item";
            label.textContent = "User: " + user + " send: " + message;
            document.getElementById("messagesList").appendChild(label);
        });

        connection.start()
            .then(function () {
                console.log("connection started!");
                document.getElementById("loginContainer").style.display = "none";
                document.getElementById("conversationContainer").style.display = "block";

            })
            .catch(error => {
                console.error(error.message);
            });

        document.getElementById("sendBtn").addEventListener("click", function (event) {
            var message = document.getElementById("userMessage").value;
            connection.invoke("SendMessage", userName, message ).catch(function (err) {
                return console.error(err.toString());
            });
            event.preventDefault();
        });
    }

    document.getElementById("loginBtn").addEventListener("click", function (event) {
        userName = document.getElementById("userName").value;
        event.preventDefault();
        startConnection();
    });

})();
