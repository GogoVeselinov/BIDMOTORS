// Dynamic CRUD Operations for Admin Panel

class CrudManager {
    constructor(apiBaseUrl, tableId, modalId) {
        this.apiBaseUrl = apiBaseUrl;
        this.tableId = tableId;
        this.modalId = modalId;
    }

    async getAll() {
        try {
            const response = await fetch(this.apiBaseUrl);
            if (!response.ok) throw new Error('Failed to fetch data');
            return await response.json();
        } catch (error) {
            console.error('Error fetching data:', error);
            this.showNotification('Грешка при зареждане на данните', 'error');
            return [];
        }
    }

    async getById(id) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/${id}`);
            if (!response.ok) throw new Error('Failed to fetch item');
            return await response.json();
        } catch (error) {
            console.error('Error fetching item:', error);
            return null;
        }
    }

    async create(data) {
        try {
            const response = await fetch(this.apiBaseUrl, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            
            if (!response.ok) throw new Error('Failed to create');
            
            const result = await response.json();
            this.showNotification(result.message || 'Успешно създаване!', 'success');
            return true;
        } catch (error) {
            console.error('Error creating item:', error);
            this.showNotification('Грешка при създаване', 'error');
            return false;
        }
    }

    async update(id, data) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/${id}`, {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(data)
            });
            
            if (!response.ok) throw new Error('Failed to update');
            
            const result = await response.json();
            this.showNotification(result.message || 'Успешна актуализация!', 'success');
            return true;
        } catch (error) {
            console.error('Error updating item:', error);
            this.showNotification('Грешка при актуализиране', 'error');
            return false;
        }
    }

    async delete(id) {
        if (!confirm('Сигурни ли сте, че искате да изтриете този запис?')) {
            return false;
        }

        try {
            const response = await fetch(`${this.apiBaseUrl}/${id}`, {
                method: 'DELETE'
            });
            
            if (!response.ok) throw new Error('Failed to delete');
            
            const result = await response.json();
            this.showNotification(result.message || 'Успешно изтриване!', 'success');
            return true;
        } catch (error) {
            console.error('Error deleting item:', error);
            this.showNotification('Грешка при изтриване', 'error');
            return false;
        }
    }

    showNotification(message, type) {
        const alertClass = type === 'success' ? 'alert-success' : 'alert-danger';
        const alertHtml = `
            <div class="alert ${alertClass} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
        
        const container = document.querySelector('.container-fluid');
        if (container) {
            const existingAlert = container.querySelector('.alert');
            if (existingAlert) existingAlert.remove();
            
            container.insertAdjacentHTML('afterbegin', alertHtml);
            
            setTimeout(() => {
                const alert = container.querySelector('.alert');
                if (alert) alert.remove();
            }, 5000);
        }
    }

    renderTable(data, columns, actions) {
        const table = document.getElementById(this.tableId);
        if (!table) return;

        const tbody = table.querySelector('tbody');
        tbody.innerHTML = '';

        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="100" class="text-center text-muted">Няма данни за показване</td></tr>';
            return;
        }

        data.forEach(item => {
            const row = document.createElement('tr');
            
            columns.forEach(col => {
                const td = document.createElement('td');
                td.textContent = col.render ? col.render(item) : item[col.field];
                row.appendChild(td);
            });

            // Actions column
            const actionsTd = document.createElement('td');
            actions.forEach(action => {
                const btn = document.createElement('button');
                btn.className = `btn btn-sm ${action.class}`;
                btn.innerHTML = `<i class="${action.icon}"></i>`;
                btn.onclick = () => action.handler(item);
                actionsTd.appendChild(btn);
            });
            row.appendChild(actionsTd);

            tbody.appendChild(row);
        });
    }
}

// Export for use in other scripts
window.CrudManager = CrudManager;
