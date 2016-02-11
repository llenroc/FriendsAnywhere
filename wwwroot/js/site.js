// Write your Javascript code.
$(function () {
    // Get handle to the chat div 
    var $chatWindow = $('#messages');

    // Manages the state of our access token we got from the server
    var accessManager;

    // Our interface to the IP Messaging service
    var messagingClient;
    
    // A handle to the "general" chat channel - the one and only channel we
    // will have in this sample app
    var generalChannel;

    // The server will assign the client a random username - store that value
    // here
    var username;

    // Helper function to print info messages to the chat window
    function print(infoMessage, asHtml) {
        var $msg = $('<div class="info">');
        if (asHtml) {
            $msg.html(infoMessage);
        } else {
            $msg.text(infoMessage);
        }
        $chatWindow.append($msg);
    }

    // Helper function to print chat message to the chat window
    function printMessage(fromUser, date, message) {
        var $user = $('<span class="username">').text(fromUser + ':');
        if (fromUser === username) {
            $user.addClass('me');
        }
        var $date = $('<span class="date">').text(moment(date).format("MMM Do YY @ HH:mm:ss"));
        var $message = $('<span class="message">').text(message);
        var $container = $('<div class="message-container">');
        $container.append($date).append('<br/>');
        $container.append($user).append($message);
        $chatWindow.append($container);
        $chatWindow.scrollTop($chatWindow[0].scrollHeight);
    }

    setTimeout(function(){ window.joinChannel('general'); }, 1000);
    
    window.joinChannel = function(chosenChannel){
        // clear div
        $chatWindow.html("");
        
        print('Logging in...');
        
        $.getJSON('/token', {
            identity: username,
            device: 'browser'
        }, function (data) {
            // Alert the user they have been assigned a random username
            username = data.identity;

            // Initialize the IP messaging client
            accessManager = new Twilio.AccessManager(data.token);
            messagingClient = new Twilio.IPMessaging.Client(accessManager);

            // Get the general chat channel, which is where all the messages are
            // sent in this simple application
            print('Attempting to join "' + chosenChannel + '" chat channel...');
            var promise = messagingClient.getChannelByUniqueName(chosenChannel);
            promise.then(function (channel) {
                generalChannel = channel;
                if (!generalChannel) {
                    // If it doesn't exist, let's create it
                    messagingClient.createChannel({
                        uniqueName: chosenChannel,
                        friendlyName: chosenChannel + ' Chat Channel'
                    }).then(function (channel) {
                        console.log('Created '+ chosenChannel + ' channel:');
                        console.log(channel);
                        generalChannel = channel;
                        setupChannel();
                    });
                } else {
                    console.log('Found' + chosenChannel + ' channel:');
                    console.log('Found' + generalChannel.Sid + ' channel:');
                    console.log(generalChannel);
                    setupChannel();
                }
            });
        });
    }

    // Set up channel after it has been found
    function setupChannel() {
        // Join the general channel
        generalChannel.join().then(function (channel) {
            print('Joined channel as '
                + '<span class="me">' + username + '</span>.', true);
                
            var promise = generalChannel.getMessages();
            promise.then(function (messages){
                for (var i=0; i < messages.length; i++){
                    printMessage(messages[i].author, messages[i].timestamp, messages[i].body);
                }
            }).catch(function(e){
                console.log(e)
            });

            // Listen for new messages sent to the channel
            generalChannel.on('messageAdded', function (message) {
                printMessage(message.author, message.timestamp, message.body);
            });
        });
    }

    // Send a new message to the general channel
    var $input = $('#chat-input');
    $input.on('keydown', function (e) {
        if (e.keyCode == 13) {
            generalChannel.sendMessage($input.val())
            $input.val('');
        }
    });
});