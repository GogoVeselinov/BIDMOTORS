// ============================================
// SERVICE PARTS MANAGER - AJAX OPERATIONS
// ============================================

let currentServiceId = null;
let availableParts = [];

/**
 * Инициализация на Parts Manager
 */
function initServicePartsManager(serviceId, parts = []) {
    currentServiceId = serviceId;
    availableParts = parts;
    
    console.log('[ServicePartsManager] Initialized with Service ID:', serviceId);
    console.log('[ServicePartsManager] Available parts:', parts.length);
    
    loadAvailableParts();
}

/**
 * Зарежда наличните части от инвентара в dropdown
 */
function loadAvailableParts() {
    fetch('/Admin/Api/Parts')
        .then(response => response.json())
        .then(data => {
            availableParts = data.filter(p => p.isActive && p.stockQuantity > 0);
            populatePartDropdown();
            console.log('[ServicePartsManager] Loaded available parts:', availableParts.length);
        })
        .catch(error => {
            console.error('[ServicePartsManager] Error loading parts:', error);
            showNotification('Грешка при зареждане на части', 'error');
        });
}

/**
 * Попълва dropdown с налични части
 */
function populatePartDropdown() {
    const select = document.getElementById('part-select');
    if (!select) return;
    
    select.innerHTML = '<option value="">-- Избери част от инвентара --</option>';
    
    availableParts.forEach(part => {
        const option = document.createElement('option');
        option.value = part.id;
        option.textContent = `${part.name} - ${part.price.toFixed(2)} лв (налични: ${part.stockQuantity})`;
        option.dataset.name = part.name;
        option.dataset.supplier = part.supplier || 'N/A';
        option.dataset.price = part.price;
        option.dataset.stock = part.stockQuantity;
        select.appendChild(option);
    });
}

/**
 * Добавя част от инвентара към Service (и автоматично към Repair)
 */
async function addPartFromInventory() {
    const partSelect = document.getElementById('part-select');
    const quantityInput = document.getElementById('part-quantity');
    
    if (!partSelect || !quantityInput) {
        console.error('[ServicePartsManager] Missing form elements');
        return;
    }
    
    const partId = partSelect.value;
    const quantity = parseInt(quantityInput.value) || 1;
    
    if (!partId) {
        showNotification('Моля изберете част', 'warning');
        return;
    }
    
    if (quantity < 1) {
        showNotification('Количеството трябва да е поне 1', 'warning');
        return;
    }
    
    const selectedOption = partSelect.options[partSelect.selectedIndex];
    const maxStock = parseInt(selectedOption.dataset.stock);
    
    if (quantity > maxStock) {
        showNotification(`Недостатъчна наличност. Максимум: ${maxStock}`, 'error');
        return;
    }
    
    try {
        console.log('[ServicePartsManager] Adding part:', partId, 'Quantity:', quantity);
        
        const response = await fetch(`/Admin/Api/Services/${currentServiceId}/parts/from-inventory`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                partId: partId,
                quantity: quantity
            })
        });
        
        const result = await response.json();
        
        if (response.ok) {
            showNotification(result.message || 'Частта беше добавена успешно', 'success');
            
            // Презареждане на списъка с части
            await loadServiceParts();
            
            // Презареждане на наличните части
            await loadAvailableParts();
            
            // Изчистване на формата
            partSelect.value = '';
            quantityInput.value = 1;
            
            // Обновяване на общата цена на ремонта (ако е видима)
            if (typeof updateRepairTotalCost === 'function') {
                updateRepairTotalCost();
            }
        } else {
            showNotification(result.message || 'Грешка при добавяне на част', 'error');
        }
    } catch (error) {
        console.error('[ServicePartsManager] Error adding part:', error);
        showNotification('Грешка при добавяне на част', 'error');
    }
}

/**
 * Зарежда списъка с части за текущия Service
 */
async function loadServiceParts() {
    try {
        const response = await fetch(`/Admin/Api/Services/${currentServiceId}`);
        const service = await response.json();
        
        displayServiceParts(service.partLinks || []);
        
    } catch (error) {
        console.error('[ServicePartsManager] Error loading service parts:', error);
    }
}

/**
 * Показва списъка с части
 */
function displayServiceParts(parts) {
    const container = document.getElementById('service-parts-list');
    if (!container) return;
    
    if (parts.length === 0) {
        container.innerHTML = '<tr><td colspan="5" class="text-center text-muted">Няма добавени части</td></tr>';
        return;
    }
    
    container.innerHTML = parts.map(part => `
        <tr>
            <td>${escapeHtml(part.title)}</td>
            <td>${escapeHtml(part.supplier || 'N/A')}</td>
            <td>${part.url ? `<a href="${escapeHtml(part.url)}" target="_blank" class="crm-link">Линк</a>` : 'N/A'}</td>
            <td>${escapeHtml(part.notes || '-')}</td>
            <td>
                <button class="btn btn-danger btn-sm" onclick="removePartFromService('${part.id}')">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}

/**
 * Премахва част от Service (и автоматично от Repair)
 */
async function removePartFromService(partLinkId) {
    if (!confirm('Сигурни ли сте? Частта ще бъде премахната и от ремонта, а наличността ще бъде възстановена.')) {
        return;
    }
    
    try {
        const response = await fetch(`/Admin/Api/Services/partlinks/${partLinkId}`, {
            method: 'DELETE'
        });
        
        const result = await response.json();
        
        if (response.ok) {
            showNotification(result.message || 'Частта беше премахната успешно', 'success');
            await loadServiceParts();
            await loadAvailableParts();
            
            if (typeof updateRepairTotalCost === 'function') {
                updateRepairTotalCost();
            }
        } else {
            showNotification(result.message || 'Грешка при премахване на част', 'error');
        }
    } catch (error) {
        console.error('[ServicePartsManager] Error removing part:', error);
        showNotification('Грешка при премахване на част', 'error');
    }
}

/**
 * Показва нотификация
 */
function showNotification(message, type = 'info') {
    const container = document.getElementById('notification-container');
    if (!container) {
        console.log('[Notification]', type.toUpperCase(), ':', message);
        return;
    }
    
    const bgClass = type === 'success' ? 'bg-success' :
                    type === 'error' ? 'bg-danger' :
                    type === 'warning' ? 'bg-warning' : 'bg-info';
    
    const notification = document.createElement('div');
    notification.className = `toast align-items-center text-white ${bgClass} border-0`;
    notification.setAttribute('role', 'alert');
    notification.innerHTML = `
        <div class="d-flex">
            <div class="toast-body">${escapeHtml(message)}</div>
            <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
        </div>
    `;
    
    container.appendChild(notification);
    
    const bsToast = new bootstrap.Toast(notification, { delay: 3000 });
    bsToast.show();
    
    notification.addEventListener('hidden.bs.toast', () => {
        notification.remove();
    });
}

/**
 * Escape HTML за безопасност
 */
function escapeHtml(text) {
    if (!text) return '';
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}
