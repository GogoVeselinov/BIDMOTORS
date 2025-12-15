// Service Types Index Page Logic
document.addEventListener('DOMContentLoaded', async () => {
    await loadServiceTypes();
    
    // Setup delete handlers
    setupDeleteHandlers();
});

async function loadServiceTypes() {
    const container = document.getElementById('service-types-container');
    
    try {
        const serviceTypes = await ServiceTypesAPI.getAll();
        
        if (serviceTypes.length === 0) {
            container.innerHTML = `
                <div class="text-center py-5 text-muted">
                    <i class="bi bi-inbox" style="font-size: 3rem;"></i>
                    <p class="mt-3">Няма добавени типове услуги</p>
                </div>
            `;
            return;
        }
        
        // Build table HTML
        const tableHtml = `
            <div class="table-responsive">
                <table class="table service-types-table align-middle">
                    <thead>
                        <tr>
                            <th>Име</th>
                            <th>Описание</th>
                            <th>Статус</th>
                            <th class="text-end">Действия</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${serviceTypes.map(type => `
                            <tr data-id="${type.id}">
                                <td>
                                    <strong class="service-type-name">${type.name}</strong>
                                </td>
                                <td class="text-muted">
                                    ${type.description || '—'}
                                </td>
                                <td>
                                    ${type.isActive ? '<span class="status-badge status-active">Активна</span>' : '<span class="status-badge status-inactive">Неактивна</span>'}
                                </td>
                                <td class="text-end">
                                    <a href="/Admin/ServiceTypes/Details/${type.id}" class="btn btn-sm btn-outline-warning">
                                        <i class="bi bi-diagram-3"></i>
                                    </a>
                                    <a href="/Admin/ServiceTypes/Edit/${type.id}" class="btn btn-sm btn-outline-light">
                                        <i class="bi bi-pencil"></i>
                                    </a>
                                    <button class="btn btn-sm btn-outline-danger delete-btn" data-id="${type.id}">
                                        <i class="bi bi-trash"></i>
                                    </button>
                                </td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        `;
        
        container.innerHTML = tableHtml;
        setupDeleteHandlers();
        
    } catch (error) {
        container.innerHTML = `
            <div class="alert alert-danger">
                <i class="bi bi-exclamation-triangle"></i>
                Грешка при зареждане на типовете услуги: ${error.message}
            </div>
        `;
    }
}

function setupDeleteHandlers() {
    document.querySelectorAll('.delete-btn').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            const id = e.currentTarget.dataset.id;
            
            if (!confirm('Сигурни ли сте, че искате да изтриете този тип услуга?')) {
                return;
            }
            
            try {
                const result = await ServiceTypesAPI.delete(id);
                showNotification(result.message, 'success');
                await loadServiceTypes();
            } catch (error) {
                showNotification(error.message, 'error');
            }
        });
    });
}
