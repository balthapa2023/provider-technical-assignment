import { useCallback, useEffect, useState } from 'react';
import { api } from '../api/client.js';
import ProviderForm from './ProviderForm.jsx';
import LicensePanel from './LicensePanel.jsx';
import { Loading, ErrorMessage, Empty } from './StatusMessage.jsx';

export default function ProviderList() {
  const [providers, setProviders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [filters, setFilters] = useState({ status: '', search: '' });
  const [editing, setEditing] = useState(null);      // null | 'new' | provider
  const [selected, setSelected] = useState(null);    // provider whose licenses are shown
  const [confirmDelete, setConfirmDelete] = useState(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try {
      const params = {};
      if (filters.status) params.status = filters.status;
      if (filters.search) params.search = filters.search;
      setProviders(await api.providers.list(params));
    } catch (e) { setError(e.message); }
    finally { setLoading(false); }
  }, [filters]);

  useEffect(() => { load(); }, [load]);

  async function handleDelete(p) {
    try {
      await api.providers.softDelete(p.providerId);
      setConfirmDelete(null);
      if (selected?.providerId === p.providerId) setSelected(null);
      load();
    } catch (e) { setError(e.message); }
  }

  return (
    <section>
      <div className="toolbar">
        <input placeholder="Search by name…" value={filters.search}
               onChange={(e) => setFilters({ ...filters, search: e.target.value })} />
        <select value={filters.status} onChange={(e) => setFilters({ ...filters, status: e.target.value })}>
          <option value="">All statuses</option>
          <option>Active</option><option>Inactive</option><option>Pending</option>
        </select>
        <button className="primary" onClick={() => setEditing('new')}>+ New Provider</button>
      </div>

      <ErrorMessage error={error} onRetry={load} />
      {loading && <Loading />}
      {!loading && !error && providers.length === 0 && <Empty>No providers match. Soft-deleted providers are hidden.</Empty>}

      {!loading && providers.length > 0 && (
        <table>
          <thead>
            <tr><th>Name</th><th>County</th><th>Status</th><th>Licenses</th><th>Expired</th><th></th></tr>
          </thead>
          <tbody>
            {providers.map((p) => (
              <tr key={p.providerId} className={selected?.providerId === p.providerId ? 'selected' : ''}>
                <td>{p.providerName}</td>
                <td>{p.county}</td>
                <td><span className={`badge ${p.status.toLowerCase()}`}>{p.status}</span></td>
                <td>{p.licenseCount}</td>
                <td>{p.expiredLicenseCount > 0 ? <span className="badge expired">{p.expiredLicenseCount}</span> : '—'}</td>
                <td className="actions">
                  <button onClick={() => setSelected(p)}>Licenses</button>
                  <button onClick={() => setEditing(p)}>Edit</button>
                  <button className="danger" onClick={() => setConfirmDelete(p)}>Delete</button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      {editing && (
        <ProviderForm
          provider={editing === 'new' ? null : editing}
          onClose={() => setEditing(null)}
          onSaved={() => { setEditing(null); load(); }}
        />
      )}

      {selected && <LicensePanel provider={selected} onClose={() => setSelected(null)} />}

      {confirmDelete && (
        <div className="modal-backdrop">
          <div className="modal">
            <h3>Delete provider?</h3>
            <p>
              <strong>{confirmDelete.providerName}</strong> will be <em>soft-deleted</em>: hidden from all
              standard views but retained in the database for audit. This can be reversed from the Audit tab.
            </p>
            <div className="modal-actions">
              <button onClick={() => setConfirmDelete(null)}>Cancel</button>
              <button className="danger" onClick={() => handleDelete(confirmDelete)}>Yes, soft-delete</button>
            </div>
          </div>
        </div>
      )}
    </section>
  );
}
