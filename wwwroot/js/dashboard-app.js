// Dashboard App - Using React with Chart.js (Showing Soft-Deleted Records)
const { useState, useEffect } = window.React;
const { Bar, Pie } = window.ChartJS2 || {};

// Summary Card Component
function SummaryCard({ icon, title, value, color, subtext }) {
    return React.createElement(
        'div',
        { className: 'col-sm-6 col-md-3 mb-3' },
        React.createElement(
            'div',
            { className: 'card' },
            React.createElement(
                'div',
                { className: 'card-body' },
                React.createElement(
                    'div',
                    { className: 'd-flex align-items-center' },
                    React.createElement(
                        'div',
                        { className: 'flex-grow-1' },
                        React.createElement('p', { className: 'text-muted small mb-1' }, title),
                        React.createElement('h3', { className: 'mb-0' }, value),
                        subtext && React.createElement('small', { className: 'text-muted mt-1 d-block' }, subtext)
                    ),
                    React.createElement(
                        'div',
                        { className: `text-${color}`, style: { fontSize: '2rem' } },
                        React.createElement('i', { className: `bi ${icon}` })
                    )
                )
            )
        )
    );
}

// Main Dashboard Component
function DashboardApp() {
    const [dashboardData, setDashboardData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchDashboardData();
    }, []);

    const fetchDashboardData = async () => {
        try {
            setLoading(true);
            console.log('Fetching dashboard data from /api/dashboard/all...');
            const response = await fetch('/api/dashboard/all');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            console.log('Dashboard data received:', data);
            setDashboardData(data);
            setError(null);

            // Initialize charts after a brief delay to ensure DOM is ready
            setTimeout(() => initializeCharts(data), 100);
        } catch (err) {
            console.error('Error fetching dashboard data:', err);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return React.createElement(
            'div',
            { className: 'alert alert-info' },
            React.createElement('p', { className: 'mb-0' },
                React.createElement('span', { className: 'spinner-border spinner-border-sm me-2' }),
                'Loading dashboard data...'
            )
        );
    }

    if (error) {
        return React.createElement(
            'div',
            { className: 'alert alert-danger' },
            React.createElement('h5', null, 'Error Loading Dashboard'),
            React.createElement('p', { className: 'mb-0' }, error),
            React.createElement('button', 
                { className: 'btn btn-sm btn-outline-danger mt-2', onClick: fetchDashboardData },
                React.createElement('i', { className: 'bi bi-arrow-clockwise me-1' }),
                'Retry'
            )
        );
    }

    if (!dashboardData) {
        return React.createElement('p', { className: 'text-muted' }, 'No data available');
    }

    const { summary, providersExpiringSoon } = dashboardData;

    return React.createElement(
        React.Fragment,
        null,
        // Alert about including deleted records
        React.createElement(
            'div',
            { className: 'alert alert-info alert-dismissible fade show mb-4' },
            React.createElement('i', { className: 'bi bi-info-circle me-2' }),
            React.createElement('strong', null, 'Note: '),
            'Dashboard includes soft-deleted records with visual indicators.',
            React.createElement(
                'button',
                { type: 'button', className: 'btn-close', 'data-bs-dismiss': 'alert' }
            )
        ),

        // Summary Cards
        React.createElement(
            'div',
            { className: 'row mb-4' },
            React.createElement(SummaryCard, {
                icon: 'bi-building',
                title: 'Total Providers',
                value: summary.totalProviders,
                color: 'primary',
                subtext: summary.deletedProviders > 0 ? `(${summary.deletedProviders} deleted)` : null
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-ticket',
                title: 'Total Licenses',
                value: summary.totalLicenses,
                color: 'info',
                subtext: summary.deletedLicenses > 0 ? `(${summary.deletedLicenses} deleted)` : null
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-exclamation-triangle',
                title: 'Expiring in 30 Days',
                value: summary.expiringIn30Days,
                color: 'warning'
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-x-circle',
                title: 'Expired Licenses',
                value: summary.expiredLicenses,
                color: 'danger'
            })
        ),

        // Charts Row
        React.createElement(
            'div',
            { className: 'row' },
            // Providers by Status
            React.createElement(
                'div',
                { className: 'col-md-6 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'Providers by Status (Include Deleted)')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'providersByStatusChart' })
                    )
                )
            ),
            // License Status
            React.createElement(
                'div',
                { className: 'col-md-6 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'License Status Distribution')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'licenseStatusChart' })
                    )
                )
            )
        ),

        // Licenses per Provider
        React.createElement(
            'div',
            { className: 'row' },
            React.createElement(
                'div',
                { className: 'col-12 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'All Licenses per Provider (Top 10, Include Deleted)')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'licensesPerProviderChart' })
                    )
                )
            )
        ),

        // Providers Expiring Soon Table
        React.createElement(
            'div',
            { className: 'row' },
            React.createElement(
                'div',
                { className: 'col-12 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' },
                            React.createElement('i', { className: 'bi bi-alarm me-2' }),
                            'Providers with Licenses Expiring Within 30 Days (Include Deleted)'
                        )
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        providersExpiringSoon && providersExpiringSoon.length > 0
                            ? React.createElement(
                                'div',
                                { className: 'table-responsive' },
                                React.createElement(
                                    'table',
                                    { className: 'table table-sm table-hover' },
                                    React.createElement(
                                        'thead',
                                        { className: 'table-light' },
                                        React.createElement(
                                            'tr',
                                            null,
                                            React.createElement('th', null, 'Provider Name'),
                                            React.createElement('th', null, 'County'),
                                            React.createElement('th', null, 'Status'),
                                            React.createElement('th', { className: 'text-end' }, 'Expiring Soon')
                                        )
                                    ),
                                    React.createElement(
                                        'tbody',
                                        null,
                                        providersExpiringSoon.map((provider, idx) =>
                                            React.createElement(
                                                'tr',
                                                { key: idx, className: provider.isDeleted ? 'table-secondary' : '' },
                                                React.createElement('td', null, 
                                                    React.createElement('strong', null, 
                                                        provider.isDeleted 
                                                            ? React.createElement(React.Fragment, null,
                                                                React.createElement('i', { className: 'bi bi-trash me-2 text-danger' }),
                                                                provider.providerName,
                                                                ' (Deleted)'
                                                              )
                                                            : provider.providerName
                                                    )
                                                ),
                                                React.createElement('td', null, provider.county),
                                                React.createElement('td', null,
                                                    React.createElement(
                                                        'span',
                                                        { className: `badge bg-${provider.status === 'Active' ? 'success' : 'warning'}` },
                                                        provider.status
                                                    )
                                                ),
                                                React.createElement(
                                                    'td',
                                                    { className: 'text-end' },
                                                    React.createElement(
                                                        'span',
                                                        { className: 'badge bg-danger' },
                                                        `${provider.expiringLicenseCount} license(s)`
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                            : React.createElement('p', { className: 'text-muted' }, 'No providers with licenses expiring within 30 days.')
                    )
                )
            )
        ),

        // Legend
        React.createElement(
            'div',
            { className: 'row mt-4' },
            React.createElement(
                'div',
                { className: 'col-12' },
                React.createElement(
                    'div',
                    { className: 'card bg-light' },
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('h6', { className: 'card-title' },
                            React.createElement('i', { className: 'bi bi-info-circle me-2' }),
                            'Legend'
                        ),
                        React.createElement('p', { className: 'small mb-1' },
                            React.createElement('i', { className: 'bi bi-trash text-danger me-2' }),
                            'Trash icon indicates soft-deleted records'
                        ),
                        React.createElement('p', { className: 'small mb-1' },
                            React.createElement('span', { className: 'badge bg-secondary' }, 'Gray row background' ),
                            ' indicates a soft-deleted provider'
                        ),
                        React.createElement('p', { className: 'small mb-0' },
                            'Soft-deleted records are kept for audit compliance but excluded from standard views'
                        )
                    )
                )
            )
        ),

        // Refresh Button
        React.createElement(
            'div',
            { className: 'row mt-4' },
            React.createElement(
                'div',
                { className: 'col-12' },
                React.createElement(
                    'button',
                    { className: 'btn btn-outline-primary', onClick: fetchDashboardData },
                    React.createElement('i', { className: 'bi bi-arrow-clockwise me-1' }),
                    'Refresh Data'
                )
            )
        )
    );
}

// Function to initialize charts after data is loaded
function initializeCharts(data) {
    if (!window.Chart) {
        console.error('Chart.js not loaded');
        return;
    }

    // Providers by Status Chart
    if (data.providersByStatus) {
        const ctx1 = document.getElementById('providersByStatusChart');
        if (ctx1) {
            ctx1._chart && ctx1._chart.destroy();
            new window.Chart(ctx1, {
                type: 'bar',
                data: {
                    labels: data.providersByStatus.labels,
                    datasets: [{
                        label: data.providersByStatus.datasets[0].label,
                        data: data.providersByStatus.datasets[0].data,
                        backgroundColor: data.providersByStatus.datasets[0].backgroundColor,
                        borderColor: data.providersByStatus.datasets[0].borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom'
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
    }

    // License Status Chart
    if (data.licenseStatus) {
        const ctx2 = document.getElementById('licenseStatusChart');
        if (ctx2) {
            ctx2._chart && ctx2._chart.destroy();
            new window.Chart(ctx2, {
                type: 'doughnut',
                data: {
                    labels: data.licenseStatus.labels,
                    datasets: [{
                        label: data.licenseStatus.datasets[0].label,
                        data: data.licenseStatus.datasets[0].data,
                        backgroundColor: data.licenseStatus.datasets[0].backgroundColor,
                        borderColor: data.licenseStatus.datasets[0].borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: true,
                            position: 'bottom'
                        }
                    }
                }
            });
        }
    }

    // Licenses per Provider Chart
    if (data.licensesPerProvider) {
        const ctx3 = document.getElementById('licensesPerProviderChart');
        if (ctx3) {
            ctx3._chart && ctx3._chart.destroy();
            new window.Chart(ctx3, {
                type: 'bar',
                data: {
                    labels: data.licensesPerProvider.labels,
                    datasets: [{
                        label: data.licensesPerProvider.datasets[0].label,
                        data: data.licensesPerProvider.datasets[0].data,
                        backgroundColor: data.licensesPerProvider.datasets[0].backgroundColor,
                        borderColor: data.licensesPerProvider.datasets[0].borderColor,
                        borderWidth: 1
                    }]
                },
                options: {
                    indexAxis: 'y',
                    responsive: true,
                    maintainAspectRatio: true,
                    plugins: {
                        legend: {
                            display: false
                        }
                    },
                    scales: {
                        x: {
                            beginAtZero: true
                        }
                    }
                }
            });
        }
    }
}

// Render the app
const root = ReactDOM.createRoot(document.getElementById('root'));

// Create a wrapper component to handle chart initialization
function AppWrapper() {
    const [dashboardData, setDashboardData] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchDashboardData();
    }, []);

    const fetchDashboardData = async () => {
        try {
            setLoading(true);
            console.log('Fetching dashboard data from /api/dashboard/all...');
            const response = await fetch('/api/dashboard/all');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const data = await response.json();
            console.log('Dashboard data received:', data);
            setDashboardData(data);
            setError(null);

            // Initialize charts after a brief delay to ensure DOM is ready
            setTimeout(() => initializeCharts(data), 100);
        } catch (err) {
            console.error('Error fetching dashboard data:', err);
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return React.createElement(
            'div',
            { className: 'alert alert-info' },
            React.createElement('p', { className: 'mb-0' },
                React.createElement('span', { className: 'spinner-border spinner-border-sm me-2' }),
                'Loading dashboard data...'
            )
        );
    }

    if (error) {
        return React.createElement(
            'div',
            { className: 'alert alert-danger' },
            React.createElement('h5', null, 'Error Loading Dashboard'),
            React.createElement('p', { className: 'mb-0' }, error),
            React.createElement('button', 
                { className: 'btn btn-sm btn-outline-danger mt-2', onClick: fetchDashboardData },
                React.createElement('i', { className: 'bi bi-arrow-clockwise me-1' }),
                'Retry'
            )
        );
    }

    if (!dashboardData) {
        return React.createElement('p', { className: 'text-muted' }, 'No data available');
    }

    const { summary, providersExpiringSoon } = dashboardData;

    return React.createElement(
        React.Fragment,
        null,
        // Alert about including deleted records
        React.createElement(
            'div',
            { className: 'alert alert-info alert-dismissible fade show mb-4' },
            React.createElement('i', { className: 'bi bi-info-circle me-2' }),
            React.createElement('strong', null, 'Note: '),
            'Dashboard includes soft-deleted records with visual indicators.',
            React.createElement(
                'button',
                { type: 'button', className: 'btn-close', 'data-bs-dismiss': 'alert' }
            )
        ),

        // Summary Cards
        React.createElement(
            'div',
            { className: 'row mb-4' },
            React.createElement(SummaryCard, {
                icon: 'bi-building',
                title: 'Total Providers',
                value: summary.totalProviders,
                color: 'primary',
                subtext: summary.deletedProviders > 0 ? `(${summary.deletedProviders} deleted)` : null
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-ticket',
                title: 'Total Licenses',
                value: summary.totalLicenses,
                color: 'info',
                subtext: summary.deletedLicenses > 0 ? `(${summary.deletedLicenses} deleted)` : null
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-exclamation-triangle',
                title: 'Expiring in 30 Days',
                value: summary.expiringIn30Days,
                color: 'warning'
            }),
            React.createElement(SummaryCard, {
                icon: 'bi-x-circle',
                title: 'Expired Licenses',
                value: summary.expiredLicenses,
                color: 'danger'
            })
        ),

        // Charts Row
        React.createElement(
            'div',
            { className: 'row' },
            // Providers by Status
            React.createElement(
                'div',
                { className: 'col-md-6 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'Providers by Status (Include Deleted)')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'providersByStatusChart' })
                    )
                )
            ),
            // License Status
            React.createElement(
                'div',
                { className: 'col-md-6 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'License Status Distribution')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'licenseStatusChart' })
                    )
                )
            )
        ),

        // Licenses per Provider
        React.createElement(
            'div',
            { className: 'row' },
            React.createElement(
                'div',
                { className: 'col-12 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' }, 'All Licenses per Provider (Top 10, Include Deleted)')
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('canvas', { id: 'licensesPerProviderChart' })
                    )
                )
            )
        ),

        // Providers Expiring Soon Table
        React.createElement(
            'div',
            { className: 'row' },
            React.createElement(
                'div',
                { className: 'col-12 mb-4' },
                React.createElement(
                    'div',
                    { className: 'card' },
                    React.createElement(
                        'div',
                        { className: 'card-header bg-light' },
                        React.createElement('h5', { className: 'mb-0' },
                            React.createElement('i', { className: 'bi bi-alarm me-2' }),
                            'Providers with Licenses Expiring Within 30 Days (Include Deleted)'
                        )
                    ),
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        providersExpiringSoon && providersExpiringSoon.length > 0
                            ? React.createElement(
                                'div',
                                { className: 'table-responsive' },
                                React.createElement(
                                    'table',
                                    { className: 'table table-sm table-hover' },
                                    React.createElement(
                                        'thead',
                                        { className: 'table-light' },
                                        React.createElement(
                                            'tr',
                                            null,
                                            React.createElement('th', null, 'Provider Name'),
                                            React.createElement('th', null, 'County'),
                                            React.createElement('th', null, 'Status'),
                                            React.createElement('th', { className: 'text-end' }, 'Expiring Soon')
                                        )
                                    ),
                                    React.createElement(
                                        'tbody',
                                        null,
                                        providersExpiringSoon.map((provider, idx) =>
                                            React.createElement(
                                                'tr',
                                                { key: idx, className: provider.isDeleted ? 'table-secondary' : '' },
                                                React.createElement('td', null, 
                                                    React.createElement('strong', null, 
                                                        provider.isDeleted 
                                                            ? React.createElement(React.Fragment, null,
                                                                React.createElement('i', { className: 'bi bi-trash me-2 text-danger' }),
                                                                provider.providerName,
                                                                ' (Deleted)'
                                                              )
                                                            : provider.providerName
                                                    )
                                                ),
                                                React.createElement('td', null, provider.county),
                                                React.createElement('td', null,
                                                    React.createElement(
                                                        'span',
                                                        { className: `badge bg-${provider.status === 'Active' ? 'success' : 'warning'}` },
                                                        provider.status
                                                    )
                                                ),
                                                React.createElement(
                                                    'td',
                                                    { className: 'text-end' },
                                                    React.createElement(
                                                        'span',
                                                        { className: 'badge bg-danger' },
                                                        `${provider.expiringLicenseCount} license(s)`
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                            : React.createElement('p', { className: 'text-muted' }, 'No providers with licenses expiring within 30 days.')
                    )
                )
            )
        ),

        // Legend
        React.createElement(
            'div',
            { className: 'row mt-4' },
            React.createElement(
                'div',
                { className: 'col-12' },
                React.createElement(
                    'div',
                    { className: 'card bg-light' },
                    React.createElement(
                        'div',
                        { className: 'card-body' },
                        React.createElement('h6', { className: 'card-title' },
                            React.createElement('i', { className: 'bi bi-info-circle me-2' }),
                            'Legend'
                        ),
                        React.createElement('p', { className: 'small mb-1' },
                            React.createElement('i', { className: 'bi bi-trash text-danger me-2' }),
                            'Trash icon indicates soft-deleted records'
                        ),
                        React.createElement('p', { className: 'small mb-1' },
                            React.createElement('span', { className: 'badge bg-secondary' }, 'Gray row background' ),
                            ' indicates a soft-deleted provider'
                        ),
                        React.createElement('p', { className: 'small mb-0' },
                            'Soft-deleted records are kept for audit compliance but excluded from standard views'
                        )
                    )
                )
            )
        ),

        // Refresh Button
        React.createElement(
            'div',
            { className: 'row mt-4' },
            React.createElement(
                'div',
                { className: 'col-12' },
                React.createElement(
                    'button',
                    { className: 'btn btn-outline-primary', onClick: fetchDashboardData },
                    React.createElement('i', { className: 'bi bi-arrow-clockwise me-1' }),
                    'Refresh Data'
                )
            )
        )
    );
}

root.render(React.createElement(AppWrapper));
