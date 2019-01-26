
(function () {
    document.getElementById("conversationContainer").style.display = "none";

    function get(url, callback) {
            var user = { Name: "Test", Password: "BdgDotNet" };
            var xhttp;
            xhttp = new XMLHttpRequest();
            xhttp.onreadystatechange = function () {
                if (this.readyState === 4 && this.status === 200) {
                    callback(this);
                }
            };
            xhttp.open("POST", "http://localhost:5000/api/auth", true);
            xhttp.setRequestHeader('Access-Control-Allow-Origin', '*');
            xhttp.setRequestHeader('Content-Type', 'application/json');
            xhttp.send(JSON.stringify(user));
    }

    document.getElementById("loginBtn").addEventListener('click', function (event) {
        var name = document.getElementById("userName").value
        var password = document.getElementById("userPassword").value
        var userDto = { Name: document.getElementById("userName").value, Password: document.getElementById("userPassword").value }
        try {
            get(userDto, connect);
        } catch (e) {
            console.log(e);
        }
    });

})();

function connect(token) {

    var connection = signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5000/chatHub", { accessTokenFactory: () => xhttp.responseText })
        .build();

    connection.on("ReceiveMessage", function (message) {
        var label = document.createElement("label");
        label.textContent = "User: " + message.User + " send: " + message.Message;
        document.getElementById("conversationContainer").appendChild(label);
    });

    connection.start()
        .then(function () {
            console.log('connection started');
            document.getElementById("loginContainer").hidden = true;
            document.getElementById("conversationContainer").hidden = false;
        })
        .catch(function (err) {
            return console.error(err.toString());
        });


}
