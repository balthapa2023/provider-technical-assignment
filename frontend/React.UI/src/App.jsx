import { useEffect, useState } from 'react'
import {
  createLicense,
  createProvider,
  deleteLicense,
  deleteProvider,
  getProvider,
  getProviders,
  updateLicense,
  updateProvider,
} from './api.js'
import './App.css'

const emptyProvider = {
  name: '',
  county: '',
  status: 'Active',
}

function getDefaultExpirationDate() {
  const date = new Date()
  date.setFullYear(date.getFullYear() + 1)

  const year = date.getFullYear()
  const month = String(date.getMonth() + 1).padStart(2, '0')
  const day = String(date.getDate()).padStart(2, '0')
  return `${year}-${month}-${day}`
}

const scenarios = [
  { value: '', label: 'All current providers' },
  {
    value: 'active-with-active-licenses',
    label: 'Active providers with active licenses',
  },
  {
    value: 'active-with-expired-licenses',
    label: 'Active providers with expired licenses',
  },
]

function App() {
  const [providers, setProviders] = useState([])
  const [selectedProvider, setSelectedProvider] = useState(null)
  const [selectedProviderId, setSelectedProviderId] = useState(null)
  const [loading, setLoading] = useState(true)
  const [loadingDetails, setLoadingDetails] = useState(false)
  const [listError, setListError] = useState('')
  const [detailsError, setDetailsError] = useState('')
  const [search, setSearch] = useState('')
  const [scenario, setScenario] = useState('')
  const [reloadKey, setReloadKey] = useState(0)
  const [detailsReloadKey, setDetailsReloadKey] = useState(0)
  const [editingProvider, setEditingProvider] = useState(null)
  const [providerToDelete, setProviderToDelete] = useState(null)
  const [editingLicense, setEditingLicense] = useState(null)
  const [licenseToDelete, setLicenseToDelete] = useState(null)
  const [saving, setSaving] = useState(false)
  const [deleting, setDeleting] = useState(false)
  const [savingLicense, setSavingLicense] = useState(false)
  const [deletingLicense, setDeletingLicense] = useState(false)
  const [formError, setFormError] = useState('')
  const [deleteError, setDeleteError] = useState('')
  const [licenseFormError, setLicenseFormError] = useState('')
  const [licenseDeleteError, setLicenseDeleteError] = useState('')

  useEffect(() => {
    const controller = new AbortController()
    const timeout = window.setTimeout(async () => {
      setLoading(true)
      setListError('')

      try {
        const result = await getProviders({
          search: search.trim(),
          scenario,
          signal: controller.signal,
        })
        setProviders(result)
      } catch (error) {
        if (error.name !== 'AbortError') {
          setListError(error.message)
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }, 250)

    return () => {
      window.clearTimeout(timeout)
      controller.abort()
    }
  }, [search, scenario, reloadKey])

  useEffect(() => {
    if (!selectedProviderId) {
      return undefined
    }

    const controller = new AbortController()
    setSelectedProvider(null)
    setDetailsError('')
    setLoadingDetails(true)

    async function loadProviderDetails() {
      try {
        const provider = await getProvider(selectedProviderId, controller.signal)
        setSelectedProvider(provider)
      } catch (error) {
        if (error.name !== 'AbortError') {
          setDetailsError(error.message)
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoadingDetails(false)
        }
      }
    }

    loadProviderDetails()
    return () => controller.abort()
  }, [selectedProviderId, detailsReloadKey])

  function showProvider(providerId) {
    setSelectedProviderId(providerId)
  }

  function openCreateForm() {
    setFormError('')
    setEditingProvider({ ...emptyProvider, id: null })
  }

  function openEditForm(provider) {
    setFormError('')
    setEditingProvider({
      id: provider.id,
      name: provider.name,
      county: provider.county,
      status: provider.status,
    })
  }

  async function saveProvider(event) {
    event.preventDefault()
    setSaving(true)
    setFormError('')

    const provider = {
      name: editingProvider.name.trim(),
      county: editingProvider.county.trim(),
      status: editingProvider.status,
    }

    try {
      const savedProvider = editingProvider.id
        ? await updateProvider(editingProvider.id, provider)
        : await createProvider(provider)

      setEditingProvider(null)
      setSelectedProviderId(savedProvider.id)
      setSelectedProvider(savedProvider)
      setReloadKey((current) => current + 1)
    } catch (error) {
      setFormError(error.message)
    } finally {
      setSaving(false)
    }
  }

  async function confirmDelete() {
    setDeleting(true)
    setDeleteError('')

    try {
      await deleteProvider(providerToDelete.id)
      if (selectedProviderId === providerToDelete.id) {
        setSelectedProviderId(null)
        setSelectedProvider(null)
      }
      setProviderToDelete(null)
      setReloadKey((current) => current + 1)
    } catch (error) {
      setDeleteError(error.message)
    } finally {
      setDeleting(false)
    }
  }

  function openCreateLicenseForm() {
    setLicenseFormError('')
    setEditingLicense({
      providerId: selectedProvider.id,
      id: null,
      number: '',
      status: 'Active',
      expirationDate: getDefaultExpirationDate(),
    })
  }

  function openEditLicenseForm(license) {
    setLicenseFormError('')
    setEditingLicense({
      providerId: selectedProvider.id,
      id: license.id,
      number: license.number,
      status: license.storedStatus,
      expirationDate: license.expirationDate,
    })
  }

  async function saveLicense(event) {
    event.preventDefault()
    setSavingLicense(true)
    setLicenseFormError('')

    const license = {
      number: editingLicense.number.trim(),
      status: editingLicense.status,
      expirationDate: editingLicense.expirationDate,
    }

    try {
      if (editingLicense.id) {
        await updateLicense(editingLicense.providerId, editingLicense.id, license)
      } else {
        await createLicense(editingLicense.providerId, license)
      }

      setEditingLicense(null)
      setReloadKey((current) => current + 1)
      setDetailsReloadKey((current) => current + 1)
    } catch (error) {
      setLicenseFormError(error.message)
    } finally {
      setSavingLicense(false)
    }
  }

  async function confirmDeleteLicense() {
    setDeletingLicense(true)
    setLicenseDeleteError('')

    try {
      await deleteLicense(licenseToDelete.providerId, licenseToDelete.license.id)
      setLicenseToDelete(null)
      setReloadKey((current) => current + 1)
      setDetailsReloadKey((current) => current + 1)
    } catch (error) {
      setLicenseDeleteError(error.message)
    } finally {
      setDeletingLicense(false)
    }
  }

  return (
    <main className="app-shell">
      <header className="topbar">
        <a className="brand" href="/" aria-label="Provider Registry home">
          <span className="brand-mark" aria-hidden="true">P</span>
          <span>Provider Registry</span>
        </a>
        <span className="topbar-caption">Operations workspace</span>
      </header>

      <section className="page-heading">
        <div>
          <p className="eyebrow">DIRECTORY</p>
          <h1>Providers</h1>
          <p className="page-description">
            Manage provider records and review their license status.
          </p>
        </div>
        <button className="button button-primary" onClick={openCreateForm}>
          <span aria-hidden="true">+</span> Add provider
        </button>
      </section>

      <section className="summary-grid" aria-label="Provider list summary">
        <SummaryCard
          label="Providers shown"
          value={loading ? '—' : providers.length}
          detail="Not soft-deleted"
        />
        <SummaryCard
          label="Active licenses"
          value={loading
            ? '—'
            : providers.reduce((total, provider) => total + provider.activeLicenseCount, 0)}
          detail="Valid today"
        />
        <SummaryCard
          label="Expired licenses"
          value={loading
            ? '—'
            : providers.reduce((total, provider) => total + provider.expiredLicenseCount, 0)}
          detail="Across providers shown"
        />
      </section>

      <section className="workspace">
        <div className="directory-panel">
          <div className="panel-heading">
            <div>
              <h2>Provider directory</h2>
              <p>Search records or filter by a license scenario.</p>
            </div>
            <span className="result-count">{providers.length} results</span>
          </div>

          <div className="directory-controls">
            <label className="search-field">
              <span className="visually-hidden">Search providers</span>
              <span aria-hidden="true" className="search-icon">⌕</span>
              <input
                type="search"
                placeholder="Search name or county"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
              />
            </label>
            <label className="scenario-select">
              <span className="visually-hidden">Filter by scenario</span>
              <select
                value={scenario}
                onChange={(event) => setScenario(event.target.value)}
              >
                {scenarios.map((option) => (
                  <option value={option.value} key={option.value}>
                    {option.label}
                  </option>
                ))}
              </select>
            </label>
          </div>

          {listError && (
            <div className="notice notice-error" role="alert">
              <strong>Could not load providers.</strong> {listError}
              <button
                className="text-button"
                onClick={() => setReloadKey((current) => current + 1)}
              >
                Try again
              </button>
            </div>
          )}

          <div className="provider-list" aria-live="polite">
            {loading && <p className="state-message">Loading providers…</p>}
            {!loading && !listError && providers.length === 0 && (
              <p className="state-message">
                No providers match this view. Try another search or add a provider.
              </p>
            )}
            {!loading && providers.map((provider) => (
              <ProviderRow
                key={provider.id}
                provider={provider}
                selected={provider.id === selectedProviderId}
                onSelect={() => showProvider(provider.id)}
                onEdit={() => openEditForm(provider)}
                onDelete={() => {
                  setDeleteError('')
                  setProviderToDelete(provider)
                }}
              />
            ))}
          </div>
        </div>

        <aside className="details-panel" aria-live="polite">
          {loadingDetails && (
            <p className="state-message">Loading provider details…</p>
          )}
          {detailsError && (
            <div className="notice notice-error" role="alert">{detailsError}</div>
          )}
          {!loadingDetails && !detailsError && !selectedProvider && (
            <div className="details-empty">
              <span className="empty-icon" aria-hidden="true">↗</span>
              <h2>Provider details</h2>
              <p>Select a provider to see its license information.</p>
            </div>
          )}
          {selectedProvider && (
            <ProviderDetails
              provider={selectedProvider}
              onEdit={() => openEditForm(selectedProvider)}
              onAddLicense={openCreateLicenseForm}
              onEditLicense={openEditLicenseForm}
              onDeleteLicense={(license) => {
                setLicenseDeleteError('')
                setLicenseToDelete({
                  providerId: selectedProvider.id,
                  providerName: selectedProvider.name,
                  license,
                })
              }}
            />
          )}
        </aside>
      </section>

      {editingProvider && (
        <ProviderForm
          provider={editingProvider}
          saving={saving}
          error={formError}
          onChange={setEditingProvider}
          onClose={() => setEditingProvider(null)}
          onSubmit={saveProvider}
        />
      )}

      {providerToDelete && (
        <DeleteConfirmation
          provider={providerToDelete}
          deleting={deleting}
          error={deleteError}
          onCancel={() => setProviderToDelete(null)}
          onConfirm={confirmDelete}
        />
      )}

      {editingLicense && (
        <LicenseForm
          license={editingLicense}
          saving={savingLicense}
          error={licenseFormError}
          onChange={setEditingLicense}
          onClose={() => setEditingLicense(null)}
          onSubmit={saveLicense}
        />
      )}

      {licenseToDelete && (
        <LicenseDeleteConfirmation
          providerName={licenseToDelete.providerName}
          license={licenseToDelete.license}
          deleting={deletingLicense}
          error={licenseDeleteError}
          onCancel={() => setLicenseToDelete(null)}
          onConfirm={confirmDeleteLicense}
        />
      )}
    </main>
  )
}

function SummaryCard({ label, value, detail }) {
  return (
    <article className="summary-card">
      <span className="summary-label">{label}</span>
      <strong className="summary-value">{value}</strong>
      <span className="summary-detail">{detail}</span>
    </article>
  )
}

function ProviderRow({ provider, selected, onSelect, onEdit, onDelete }) {
  return (
    <article className={`provider-row${selected ? ' selected' : ''}`}>
      <button className="provider-main" onClick={onSelect}>
        <span className="provider-avatar" aria-hidden="true">
          {provider.name.slice(0, 1).toUpperCase()}
        </span>
        <span className="provider-copy">
          <strong>{provider.name}</strong>
          <span>{provider.county} County</span>
        </span>
      </button>
      <span className={`status-pill status-${provider.status.toLowerCase()}`}>
        {provider.status}
      </span>
      <span className="license-count">
        {provider.licenseCount} {provider.licenseCount === 1 ? 'license' : 'licenses'}
      </span>
      <div className="row-actions">
        <button className="icon-button" onClick={onEdit} aria-label={`Edit ${provider.name}`}>
          Edit
        </button>
        <button
          className="icon-button icon-button-danger"
          onClick={onDelete}
          aria-label={`Delete ${provider.name}`}
        >
          Delete
        </button>
      </div>
    </article>
  )
}

function ProviderDetails({
  provider,
  onEdit,
  onAddLicense,
  onEditLicense,
  onDeleteLicense,
}) {
  return (
    <div className="provider-details">
      <div className="details-heading">
        <div>
          <p className="eyebrow">PROVIDER PROFILE</p>
          <h2>{provider.name}</h2>
          <p>{provider.county} County</p>
        </div>
        <button className="button button-secondary" onClick={onEdit}>Edit</button>
      </div>

      <div className="detail-meta">
        <span>Status</span>
        <span className={`status-pill status-${provider.status.toLowerCase()}`}>
          {provider.status}
        </span>
      </div>

      <div className="license-heading">
        <div>
          <h3>Licenses</h3>
          <p>{provider.licenses.length} associated with this provider</p>
        </div>
        <button className="button button-secondary" onClick={onAddLicense}>
          Add license
        </button>
      </div>

      {provider.licenses.length === 0 ? (
        <p className="license-empty">No licenses have been recorded.</p>
      ) : (
        <ul className="license-list">
          {provider.licenses.map((license) => (
            <li className="license-card" key={license.id}>
              <div className="license-card-main">
                <strong>{license.number}</strong>
                <span>Expires {formatDate(license.expirationDate)}</span>
              </div>
              <div className="license-card-actions">
                <span className={`license-status license-${license.status.toLowerCase()}`}>
                  {license.status}
                </span>
                <button
                  className="icon-button"
                  onClick={() => onEditLicense(license)}
                  aria-label={`Edit license ${license.number}`}
                >
                  Edit
                </button>
                <button
                  className="icon-button icon-button-danger"
                  onClick={() => onDeleteLicense(license)}
                  aria-label={`Delete license ${license.number}`}
                >
                  Delete
                </button>
              </div>
            </li>
          ))}
        </ul>
      )}
    </div>
  )
}

function ProviderForm({ provider, saving, error, onChange, onClose, onSubmit }) {
  const isEditing = Boolean(provider.id)

  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="provider-form-title"
      >
        <div className="modal-heading">
          <div>
            <p className="eyebrow">{isEditing ? 'UPDATE RECORD' : 'NEW RECORD'}</p>
            <h2 id="provider-form-title">
              {isEditing ? 'Edit provider' : 'Add provider'}
            </h2>
          </div>
          <button className="close-button" onClick={onClose} aria-label="Close form">
            ×
          </button>
        </div>
        <form onSubmit={onSubmit}>
          <label className="form-field">
            <span>Provider name</span>
            <input
              required
              minLength="2"
              maxLength="120"
              value={provider.name}
              onChange={(event) => onChange({ ...provider, name: event.target.value })}
              autoFocus
            />
          </label>
          <label className="form-field">
            <span>County</span>
            <input
              required
              minLength="2"
              maxLength="80"
              value={provider.county}
              onChange={(event) => onChange({ ...provider, county: event.target.value })}
            />
          </label>
          <label className="form-field">
            <span>Provider status</span>
            <select
              value={provider.status}
              onChange={(event) => onChange({ ...provider, status: event.target.value })}
            >
              <option>Active</option>
              <option>Inactive</option>
              <option>Pending</option>
            </select>
          </label>
          {error && <div className="notice notice-error" role="alert">{error}</div>}
          <div className="modal-actions">
            <button type="button" className="button button-secondary" onClick={onClose}>
              Cancel
            </button>
            <button className="button button-primary" disabled={saving}>
              {saving ? 'Saving…' : isEditing ? 'Save changes' : 'Create provider'}
            </button>
          </div>
        </form>
      </section>
    </div>
  )
}

function LicenseForm({ license, saving, error, onChange, onClose, onSubmit }) {
  const isEditing = Boolean(license.id)

  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="modal"
        role="dialog"
        aria-modal="true"
        aria-labelledby="license-form-title"
      >
        <div className="modal-heading">
          <div>
            <p className="eyebrow">{isEditing ? 'UPDATE RECORD' : 'NEW RECORD'}</p>
            <h2 id="license-form-title">
              {isEditing ? 'Edit license' : 'Add license'}
            </h2>
          </div>
          <button className="close-button" onClick={onClose} aria-label="Close form">
            ×
          </button>
        </div>
        <form onSubmit={onSubmit}>
          <label className="form-field">
            <span>License number</span>
            <input
              required
              minLength="2"
              maxLength="40"
              value={license.number}
              onChange={(event) => onChange({ ...license, number: event.target.value })}
              autoFocus
            />
          </label>
          <label className="form-field">
            <span>License status</span>
            <select
              value={license.status}
              onChange={(event) => onChange({ ...license, status: event.target.value })}
            >
              <option>Active</option>
              <option>Suspended</option>
            </select>
          </label>
          <label className="form-field">
            <span>Expiration date</span>
            <input
              required
              type="date"
              value={license.expirationDate}
              onChange={(event) => onChange({
                ...license,
                expirationDate: event.target.value,
              })}
            />
          </label>
          {error && <div className="notice notice-error" role="alert">{error}</div>}
          <div className="modal-actions">
            <button type="button" className="button button-secondary" onClick={onClose}>
              Cancel
            </button>
            <button className="button button-primary" disabled={saving}>
              {saving ? 'Saving…' : isEditing ? 'Save changes' : 'Create license'}
            </button>
          </div>
        </form>
      </section>
    </div>
  )
}

function DeleteConfirmation({ provider, deleting, error, onCancel, onConfirm }) {
  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="modal modal-confirmation"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="delete-title"
        aria-describedby="delete-description"
      >
        <div className="confirmation-icon" aria-hidden="true">!</div>
        <h2 id="delete-title">Remove this provider?</h2>
        <p id="delete-description">
          <strong>{provider.name}</strong> will be hidden from the directory. Its
          record and license history will be retained for audit purposes.
        </p>
        {error && <div className="notice notice-error" role="alert">{error}</div>}
        <div className="modal-actions">
          <button className="button button-secondary" onClick={onCancel} disabled={deleting}>
            Keep provider
          </button>
          <button
            className="button button-danger"
            onClick={onConfirm}
            disabled={deleting}
          >
            {deleting ? 'Removing…' : 'Soft delete provider'}
          </button>
        </div>
      </section>
    </div>
  )
}

function LicenseDeleteConfirmation({
  providerName,
  license,
  deleting,
  error,
  onCancel,
  onConfirm,
}) {
  return (
    <div className="modal-backdrop" role="presentation">
      <section
        className="modal modal-confirmation"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby="delete-license-title"
        aria-describedby="delete-license-description"
      >
        <div className="confirmation-icon" aria-hidden="true">!</div>
        <h2 id="delete-license-title">Remove this license?</h2>
        <p id="delete-license-description">
          License <strong>{license.number}</strong> will be hidden from the license
          list for {providerName}. Its record will be retained for audit purposes.
        </p>
        {error && <div className="notice notice-error" role="alert">{error}</div>}
        <div className="modal-actions">
          <button className="button button-secondary" onClick={onCancel} disabled={deleting}>
            Keep license
          </button>
          <button
            className="button button-danger"
            onClick={onConfirm}
            disabled={deleting}
          >
            {deleting ? 'Removing…' : 'Soft delete license'}
          </button>
        </div>
      </section>
    </div>
  )
}

function formatDate(date) {
  return new Date(`${date}T00:00:00`).toLocaleDateString(undefined, {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
  })
}

export default App
