// Basic API configuration
const API_URL = '/api'; // Same host

let gameState = {
    cash: 1000.00,
    animals: [
        { id: 1, name: 'Bessy', age: 2, type: 'Cow', progress: 45 },
        { id: 2, name: 'Molly', age: 1, type: 'Sheep', progress: 80 }
    ],
    products: [
        { type: 'Milk', quantity: 15, price: 12.50 },
        { type: 'Wool', quantity: 5, price: 45.00 }
    ]
};

function render() {
    // Update Cash
    document.getElementById('cashDisplay').innerText = `Cash: ${gameState.cash.toFixed(2)} TL`;

    // Update Animals Table
    const animalsBody = document.querySelector('#animalsTable tbody');
    animalsBody.innerHTML = gameState.animals.map(a => `
        <tr>
            <td>${a.id}</td>
            <td>${a.name}</td>
            <td>${a.age}</td>
            <td>${a.type}</td>
            <td>
                <div class="progress-bar-container">
                    <div class="progress-bar" style="width: ${a.progress}%"></div>
                </div>
            </td>
        </tr>
    `).join('');

    // Update Products Table
    const productsBody = document.querySelector('#productsTable tbody');
    productsBody.innerHTML = gameState.products.map(p => `
        <tr>
            <td>${p.type}</td>
            <td>${p.quantity}</td>
            <td>${p.price.toFixed(2)} TL</td>
        </tr>
    `).join('');
}

function buyAnimal() {
    console.log("Buy Animal");
    // Fetch from API in future
}

function sellAnimal() {
    console.log("Sell Animal");
}

function sellProducts() {
    console.log("Sell Products");
}

function resetGame() {
    if(confirm('Reset game?')) {
        console.log("Resetting...");
    }
}

// Initial render
render();
