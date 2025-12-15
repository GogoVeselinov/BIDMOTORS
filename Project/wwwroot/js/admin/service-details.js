let serviceTypeId = '';
let serviceTypeName = '';
let isActive = false;
let tasks = [];
let parts = [];

function initServiceDetails(id, name, active, tasksData, partsData) {
    serviceTypeId = id;
    serviceTypeName = name;
    isActive = active;
    tasks = tasksData || [];
    parts = partsData || [];
    
    renderTasks();
    renderParts();
}

document.addEventListener('DOMContentLoaded', () => {
    // Data will be initialized from the view
});

async function deleteServiceType() {
    if (!confirm('Сигурни ли сте, че искате да изтриете този тип услуга?')) {
        return;
    }
    
    try {
        await ServiceTypesAPI.delete(serviceTypeId);
        showNotification('Типът услуга е изтрит успешно!', 'success');
        setTimeout(() => {
            window.location.href = '/Admin/ServiceTypes/Index';
        }, 1500);
    } catch (error) {
        showNotification('Грешка при изтриване: ' + error.message, 'danger');
    }
}


async function addTask() {
    const input = document.getElementById('new-task-input');
    const title = input.value.trim();
    
    if (!title) {
        showNotification('Въведете име на стъпката', 'warning');
        return;
    }
    
    try {
        const response = await fetch(`/api/admin/servicetypes/${serviceTypeId}/tasks`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ title })
        });
        
        if (response.ok) {
            showNotification('Стъпката е добавена', 'success');
            input.value = '';
            location.reload();
        } else {
            showNotification('Грешка при добавяне', 'danger');
        }
    } catch (error) {
        showNotification('Грешка: ' + error.message, 'danger');
    }
}



async function deleteTask(taskId) {
    if (!confirm('Изтрий стъпката?')) return;
    
    try {
        const response = await fetch(`/api/admin/servicetypes/tasks/${taskId}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            tasks = tasks.filter(t => t.Id !== taskId);
            renderTasks();
            showNotification('Стъпката е изтрита', 'info');
        } else {
            showNotification('Грешка при изтриване', 'danger');
        }
    } catch (error) {
        showNotification('Грешка: ' + error.message, 'danger');
    }
}
        
function renderTasks() {
    const list = document.getElementById('tasks-list');
    
    if (tasks.length === 0) {
        list.innerHTML = '<li class="text-muted">Няма добавени стъпки за този тип услуга</li>';
        return;
    }
    
    list.innerHTML = tasks.map(task => `
        <li>
            <input type="checkbox" ${task.IsCompleted ? 'checked' : ''} onchange="toggleTaskCompleted('${task.Id}', this.checked)" />
            <span>${task.Title}</span>
            <button class="btn btn-sm btn-danger ms-auto" onclick="deleteTask('${task.Id}')" style="padding: 2px 8px;">
                <i class="bi bi-x"></i>
            </button>
        </li>
    `).join('');
}

async function toggleTaskCompleted(taskId, isCompleted) {
    try {
        const response = await fetch(`/api/admin/servicetypes/tasks/${taskId}/completed`, {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ isCompleted })
        });
        
        if (response.ok) {
            // Update local state
            const task = tasks.find(t => t.Id === taskId);
            if (task) {
                task.IsCompleted = isCompleted;
            }
            showNotification(isCompleted ? 'Стъпката е маркирана' : 'Стъпката е демаркирана', 'success');
        } else {
            showNotification('Грешка при обновяване', 'danger');
            // Revert checkbox
            renderTasks();
        }
    } catch (error) {
        showNotification('Грешка: ' + error.message, 'danger');
        renderTasks();
    }
}
        
async function addPart() {
    const title = document.getElementById('new-part-title').value.trim();
    const supplier = document.getElementById('new-part-supplier').value.trim();
    const url = document.getElementById('new-part-url').value.trim();
    
    if (!title || !url) {
        showNotification('Въведете име и линк', 'warning');
        return;
    }
    
    try {
        const response = await fetch(`/api/admin/servicetypes/${serviceTypeId}/parts`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ title, url, supplier })
        });
        
        if (response.ok) {
            showNotification('Частта е добавена', 'success');
            document.getElementById('new-part-title').value = '';
            document.getElementById('new-part-supplier').value = '';
            document.getElementById('new-part-url').value = '';
            location.reload();
        } else {
            showNotification('Грешка при добавяне', 'danger');
        }
    } catch (error) {
        showNotification('Грешка: ' + error.message, 'danger');
    }
}
        
async function deletePart(partId) {
    if (!confirm('Изтрий частта?')) return;
    
    try {
        const response = await fetch(`/api/admin/servicetypes/parts/${partId}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            parts = parts.filter(p => p.Id !== partId);
            renderParts();
            showNotification('Частта е изтрита', 'info');
        } else {
            showNotification('Грешка при изтриване', 'danger');
        }
    } catch (error) {
        showNotification('Грешка: ' + error.message, 'danger');
    }
}
        
function renderParts() {
    const tbody = document.getElementById('parts-list');
    
    if (parts.length === 0) {
        tbody.innerHTML = '<tr><td colspan="4" class="text-white text-center">Няма добавени части</td></tr>';
        return;
    }
    
    tbody.innerHTML = parts.map(part => `
        <tr>
            <td>${part.Title}</td>
            <td>${part.Supplier || '-'}</td>
            <td><a href="${part.Url}" target="_blank" class="text-warning">${part.Url}</a></td>
            <td>
                <button class="btn btn-sm btn-danger" onclick="deletePart('${part.Id}')">
                    <i class="bi bi-trash"></i>
                </button>
            </td>
        </tr>
    `).join('');
}
        
async function saveNotes() {
    const notes = document.getElementById('notes-area').value.trim();
    
    try {
        const updateData = {
            id: serviceTypeId,
            name: serviceTypeName,
            description: notes,
            isActive: isActive
        };
        
        await ServiceTypesAPI.update(serviceTypeId, updateData);
        showNotification('Бележките са запазени успешно!', 'success');
    } catch (error) {
        showNotification('Грешка при запазване: ' + error.message, 'danger');
    }
}