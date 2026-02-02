const API_URL = '/api';
let currentFarmId = null;
let selectedAnimalIds = []; // Changed to an array for multiple selection
let gameState = {
    user: null,
    animals: [],
    products: []
};

// Token Check
const token = localStorage.getItem('token');
if (!token) {
    window.location.href = 'login.html';
}

// UI Notification System
function showNotification(message, type = 'info') {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    const toast = document.createElement('div');
    toast.className = `toast ${type}`;
    toast.innerText = message;
    container.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// Custom Confirmation Modal
function showConfirm(title, message, onConfirm) {
    const modal = document.getElementById('messageModal');
    if (!modal) return;

    document.getElementById('msgModalTitle').innerText = title;
    document.getElementById('msgModalContent').innerText = message;
    
    const okBtn = document.getElementById('msgModalOkBtn');
    const newOkBtn = okBtn.cloneNode(true);
    okBtn.parentNode.replaceChild(newOkBtn, okBtn);
    
    newOkBtn.onclick = () => {
        window.closeMsgModal();
        onConfirm();
    };

    modal.style.display = 'flex';
}

window.closeMsgModal = function() {
    const modal = document.getElementById('messageModal');
    if (modal) modal.style.display = 'none';
};

// Helpers
function fixDate(dateStr) {
    if (!dateStr) return null;
    return new Date(dateStr.endsWith('Z') ? dateStr : dateStr + 'Z');
}

async function request(endpoint, options = {}) {
    const headers = {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
        ...options.headers
    };

    try {
        const response = await fetch(`${API_URL}${endpoint}`, { ...options, headers });
        
        if (response.status === 401 || response.status === 404) {
            localStorage.removeItem('token');
            window.location.href = 'login.html';
            return;
        }

        if (!response.ok) {
            const errorText = await response.text();
            let errorMsg = errorText;
            try {
                const errorJson = JSON.parse(errorText);
                errorMsg = errorJson.message || errorJson;
            } catch(e) {}
            throw new Error(errorMsg || 'Request failed');
        }

        return response.status !== 204 ? await response.json() : true;
    } catch (err) {
        console.error("API Error:", err);
        showNotification(err.message, 'error');
    }
}

// Animal Selection Logic (Multiple and Toggle)
window.selectAnimal = function(id) {
    const index = selectedAnimalIds.indexOf(id);
    if (index > -1) {
        // If already selected, remove it (Toggle off)
        selectedAnimalIds.splice(index, 1);
    } else {
        // If not selected, add to array
        selectedAnimalIds.push(id);
    }
    renderAnimals();
    updateButtonLabels();
};

function updateButtonLabels() {
    const sellBtn = document.querySelector('button[onclick="sellAnimal()"]');
    if (sellBtn) {
        const icon = '<i class="fa-solid fa-money-bill-transfer"></i> ';
        if (selectedAnimalIds.length > 1) {
            sellBtn.innerHTML = `${icon} Sell Selected (${selectedAnimalIds.length})`;
        } else {
            sellBtn.innerHTML = `${icon} Sell Animal`;
        }
    }
}

// --- Modal Logic ---
window.buyAnimal = function() {
    const modal = document.getElementById('buyAnimalModal');
    if (modal) {
        modal.style.display = 'flex';
        document.getElementById('newAnimalName').value = '';
        updatePriceLabel();
    }
};

window.closeBuyModal = function() {
    const modal = document.getElementById('buyAnimalModal');
    if (modal) modal.style.display = 'none';
};

window.updatePriceLabel = function() {
    const type = document.getElementById('newAnimalType').value;
    let price = 0;
    if (type === "Cow") price = 500;
    else if (type === "Sheep") price = 200;
    else if (type === "Chicken") price = 30;
    
    const label = document.getElementById('buyPriceLabel');
    if (label) label.innerText = `${price.toFixed(2)} TL`;
};

window.confirmBuyAnimal = async function() {
    const type = document.getElementById('newAnimalType').value;
    const name = document.getElementById('newAnimalName').value;
    const species = type.charAt(0).toUpperCase() + type.slice(1).toLowerCase();

    let price = 0, interval = 0;
    if (species === "Cow") { price = 500; interval = 30; }
    else if (species === "Sheep") { price = 200; interval = 20; }
    else if (species === "Chicken") { price = 30; interval = 10; }
    else { showNotification("Invalid species!", "error"); return; }

    const result = await request(`/farms/${currentFarmId}/animals/buy`, {
        method: 'POST',
        body: JSON.stringify({ 
            species: species, 
            name: name || species,
            purchasePrice: price,
            productionInterval: interval
        })
    });
    
    if (result) {
        showNotification(`${name || species} bought successfully!`, 'success');
        closeBuyModal();
        await refreshData();
    }
};

// Actions
window.sellAnimal = async function() {
    if (selectedAnimalIds.length === 0) {
        showNotification("Please select at least one animal first", 'error');
        return;
    }

    const count = selectedAnimalIds.length;
    const message = count > 1 
        ? `Are you sure you want to sell ${count} selected animals?`
        : `Are you sure you want to sell this animal?`;

    showConfirm("Sell Animals", message, async () => {
        let successCount = 0;
        // Process each selected animal
        for (const animalId of [...selectedAnimalIds]) {
            const result = await request(`/animals/${animalId}/sell`, { method: 'POST' });
            if (result) successCount++;
        }

        if (successCount > 0) {
            showNotification(`${successCount} animals sold!`, 'success');
            selectedAnimalIds = [];
            updateButtonLabels();
            await refreshData();
        }
    });
};

window.sellProducts = async function() {
    if (!currentFarmId) return;
    
    const totalVal = gameState.products.reduce((sum, p) => sum + (p.salePrice * p.quantity), 0);
    if (totalVal <= 0) {
        showNotification("No products to sell!", "info");
        return;
    }

    showConfirm("Sell All Products", `Sell all products for ${totalVal.toFixed(2)} TL?`, async () => {
        const result = await request(`/farms/${currentFarmId}/products/sell-all`, { method: 'POST' });
        if(result) showNotification(`Sold everything for ${result.totalEarnings.toFixed(2)} TL`, 'success');
        await refreshData();
    });
};

window.resetGame = async function() {
    showConfirm("Reset Game", "CRITICAL: This will delete ALL progress. Continue?", async () => {
        const result = await request('/users/reset', { method: 'POST' });
        if (result) {
            showNotification("Game has been reset.", 'success');
            setTimeout(() => location.reload(), 1000);
        }
    });
};

async function init() {
    const user = await request('/users/me');
    if (!user) return;

    gameState.user = user;
    updateBalanceDisplay();

    const farms = await request('/farms');
    if (farms && farms.length > 0) {
        currentFarmId = farms[0].id;
        await refreshData();
    }
}

function updateBalanceDisplay() {
    const display = document.getElementById('cashDisplay');
    if (gameState.user && display) {
        display.innerHTML = `<i class="fa-solid fa-wallet"></i> ${gameState.user.balance.toFixed(2)} TL`;
    }
}

async function refreshData() {
    if (!currentFarmId) return;

    const [animals, products, balanceData] = await Promise.all([
        request(`/farms/${currentFarmId}/animals`),
        request(`/farms/${currentFarmId}/products`),
        request('/users/me/balance')
    ]);

    if (animals) gameState.animals = animals;
    if (products) gameState.products = products;
    if (balanceData && gameState.user) gameState.user.balance = balanceData.balance;
    
    renderAnimals();
    renderProducts();
    updateBalanceDisplay();
}

function renderAnimals() {
    const body = document.querySelector('#animalsTable tbody');
    if (!body) return;
    
    const now = new Date();

    body.innerHTML = gameState.animals.map(a => {
        const birthDate = fixDate(a.birthDate);
        if(!birthDate) return '';
        const diffSeconds = Math.max(0, (now.getTime() - birthDate.getTime()) / 1000);
        const ageYears = Math.floor(diffSeconds / 30);
        
        let prog = 0;
        if (a.nextProductionAt && a.productionInterval > 0) {
            const nextProd = fixDate(a.nextProductionAt);
            if (nextProd <= now) {
                prog = 100;
            } else {
                const remainingSeconds = (nextProd.getTime() - now.getTime()) / 1000;
                const ratio = 1.0 - (remainingSeconds / a.productionInterval);
                prog = Math.floor(ratio * 100);
                prog = Math.max(0, Math.min(100, prog));
            }
        }

        const isSelected = selectedAnimalIds.includes(a.id);

        return `
            <tr onclick="selectAnimal('${a.id}')" class="${isSelected ? 'selected-row' : ''}">
                <td>${a.id.substring(0, 8)}</td>
                <td>${a.name}</td>
                <td>${ageYears} Years</td>
                <td>${a.species}</td>
                <td>
                    <div class="progress-bar-container">
                        <div class="progress-bar" style="width: ${prog}%"></div>
                    </div>
                </td>
            </tr>
        `;
    }).join('');
}

function renderProducts() {
    const body = document.querySelector('#productsTable tbody');
    if (!body) return;

    const grouped = gameState.products.reduce((acc, p) => {
        if (!acc[p.productType]) {
            acc[p.productType] = { type: p.productType, quantity: 0, price: p.salePrice };
        }
        acc[p.productType].quantity += p.quantity;
        return acc;
    }, {});

    body.innerHTML = Object.values(grouped).map(p => `
        <tr>
            <td>${p.type}</td>
            <td>${p.quantity}</td>
            <td>${(p.price * p.quantity).toFixed(2)} TL</td>
        </tr>
    `).join('');
}

document.addEventListener('DOMContentLoaded', init);
setInterval(refreshData, 1000);
