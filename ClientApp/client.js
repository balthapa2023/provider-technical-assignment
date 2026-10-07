// Single place where the React UI talks to the C# API.
async function request(url, options = {}) {
  const res = await fetch(url, {
    headers: { 'Content-Type': 'application/json' },
    ...options
  });

  if (res.status === 204) return null;

  const body = await res.json().catch(() => null);
  if (!res.ok) {
    // ASP.NET Core returns ProblemDetails / ValidationProblemDetails
    const message =
      body?.detail ||
      body?.title ||
      (body?.errors && Object.values(body.errors).flat().join(' ')) ||
      `Request failed (${res.status})`;
    throw new Error(message);
  }
  return body;
}

export const api = {
  providers: {
    list: (params = {}) => request('/api/providers?' + new URLSearchParams(params)),
    get: (id) => request(`/api/providers/${id}`),
    create: (data) => request('/api/providers', { method: 'POST', body: JSON.stringify(data) }),
    update: (id, data) => request(`/api/providers/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
    softDelete: (id) => request(`/api/providers/${id}`, { method: 'DELETE' }),
    deleted: () => request('/api/providers/audit/deleted'),
    restore: (id) => request(`/api/providers/${id}/restore`, { method: 'POST' })
  },
  licenses: {
    list: (providerId) => request(`/api/providers/${providerId}/licenses`),
    create: (providerId, data) =>
      request(`/api/providers/${providerId}/licenses`, { method: 'POST', body: JSON.stringify(data) })
  },
  reports: {
    activeActive: () => request('/api/reports/active-providers-active-licenses'),
    activeExpired: () => request('/api/reports/active-providers-expired-licenses'),
    dashboard: () => request('/api/reports/dashboard')
  }
};
