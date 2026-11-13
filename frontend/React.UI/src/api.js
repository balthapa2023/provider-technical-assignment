async function request(path, options = {}) {
  const response = await fetch(`/api${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options.headers,
    },
  })

  if (response.status === 204) {
    return null
  }

  const responseBody = await response.text()
  const result = responseBody ? JSON.parse(responseBody) : {}

  if (!response.ok) {
    const validationMessages = result.errors
      ? Object.values(result.errors).flat()
      : []
    const message = validationMessages.length > 0
      ? validationMessages.join(' ')
      : result.detail || result.title || 'The request could not be completed.'

    throw new Error(message)
  }

  return result
}

export function getProviders({ search, scenario, signal }) {
  const query = new URLSearchParams()

  if (search) {
    query.set('search', search)
  }

  if (scenario) {
    query.set('scenario', scenario)
  }

  const queryString = query.toString()
  const path = queryString ? `/providers?${queryString}` : '/providers'

  return request(path, { signal })
}

export function getProvider(id, signal) {
  return request(`/providers/${id}`, { signal })
}

export function createProvider(provider) {
  return request('/providers', {
    method: 'POST',
    body: JSON.stringify(provider),
  })
}

export function updateProvider(id, provider) {
  return request(`/providers/${id}`, {
    method: 'PUT',
    body: JSON.stringify(provider),
  })
}

export function deleteProvider(id) {
  return request(`/providers/${id}`, { method: 'DELETE' })
}

export function createLicense(providerId, license) {
  return request(`/providers/${providerId}/licenses`, {
    method: 'POST',
    body: JSON.stringify(license),
  })
}

export function updateLicense(providerId, licenseId, license) {
  return request(`/providers/${providerId}/licenses/${licenseId}`, {
    method: 'PUT',
    body: JSON.stringify(license),
  })
}

export function deleteLicense(providerId, licenseId) {
  return request(`/providers/${providerId}/licenses/${licenseId}`, {
    method: 'DELETE',
  })
}
