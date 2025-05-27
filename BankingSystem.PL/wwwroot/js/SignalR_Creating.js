// Establish SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/create")
    .withAutomaticReconnect()
    .build();

// Start the connection
async function startConnection() {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(startConnection, 5000);
    }
}

// Handle customer creation response
connection.on("ReceiveCustomerCreated", (customerId, tellerId) => {
    console.log(`Customer ${customerId} created successfully by teller ${tellerId}`);
    // Redirect after a short delay
    setTimeout(() => {
        window.location.href = `/HandleCustomer/GetAllCustomers/${tellerId}`;
    }, 2000);
});

// Initialize when document is ready
$(document).ready(function () {
    // Start SignalR connection
    startConnection();

    $('form').on('submit', function (e) {
        e.preventDefault();

        $.ajax({
            url: $(this).attr('action'),
            method: 'POST',
            data: $(this).serialize()
        });
    });
});