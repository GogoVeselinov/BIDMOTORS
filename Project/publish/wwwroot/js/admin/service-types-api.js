// Service Types API Client
const ServiceTypesAPI = {
    baseUrl: '/api/admin/servicetypes',

    // GET all service types
    async getAll() {
        try {
            const response = await fetch(this.baseUrl);
            if (!response.ok) throw new Error('Failed to fetch service types');
            return await response.json();
        } catch (error) {
            console.error('Error fetching service types:', error);
            throw error;
        }
    },

    // GET service type by ID
    async getById(id) {
        try {
            const response = await fetch(`${this.baseUrl}/${id}`);
            if (!response.ok) throw new Error('Service type not found');
            return await response.json();
        } catch (error) {
            console.error('Error fetching service type:', error);
            throw error;
        }
    },

    // POST create new service type
    async create(data) {
        try {
            const response = await fetch(this.baseUrl, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });
            
            const result = await response.json();
            if (!response.ok) throw new Error(result.message || 'Failed to create service type');
            return result;
        } catch (error) {
            console.error('Error creating service type:', error);
            throw error;
        }
    },

    // PUT update service type
    async update(id, data) {
        try {
            const response = await fetch(`${this.baseUrl}/${id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(data)
            });
            
            const result = await response.json();
            if (!response.ok) throw new Error(result.message || 'Failed to update service type');
            return result;
        } catch (error) {
            console.error('Error updating service type:', error);
            throw error;
        }
    },

    // DELETE service type
    async delete(id) {
        try {
            const response = await fetch(`${this.baseUrl}/${id}`, {
                method: 'DELETE'
            });
            
            const result = await response.json();
            if (!response.ok) throw new Error(result.message || 'Failed to delete service type');
            return result;
        } catch (error) {
            console.error('Error deleting service type:', error);
            throw error;
        }
    }
};

// Helper function to show notifications
function showNotification(message, type = 'success') {
    const notificationHtml = `
        <div class="alert alert-${type === 'success' ? 'success' : 'danger'} alert-dismissible fade show notification-toast" role="alert">
            <i class="bi bi-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </div>
    `;
    
    const container = document.querySelector('.notification-container') || document.body;
    container.insertAdjacentHTML('afterbegin', notificationHtml);
    
    // Auto-remove after 5 seconds
    setTimeout(() => {
        const alert = container.querySelector('.notification-toast');
        if (alert) {
            alert.classList.remove('show');
            setTimeout(() => alert.remove(), 150);
        }
    }, 5000);
}

// Export for use in other scripts
window.ServiceTypesAPI = ServiceTypesAPI;
window.showNotification = showNotification;
