import { useCallback, useEffect, useState } from 'react';
import { api } from '../api/client.js';
import { Loading, ErrorMessage, Empty } from './StatusMessage.jsx';

export default function LicensePanel({ provider, onClose }) {
  const [licenses, setLicenses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [form, setForm] = useState({ licenseNumber: '', licenseStatus: 'Active', expirationDate: '' });
  const [formError, setFormError] = useState(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try { setLicenses(await api.licenses.list(provider.providerId)); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  }, [provider.providerId]);

  useEffect(() => { load(); }, [load]);

  async function addLicense(ev) {
    ev.preventDefault();
    setFormError(null);
    if (!form.licenseNumber.trim() || !form.expirationDate) {
      setFormError('License number and expiration date are required.');
      return;
    }
    try {
      await api.licenses.create(provider.providerId, form);
      setForm({ licenseNumber: '', licenseStatus: 'Active', expirationDate: '' });
      load();
    } catch (e) { setFormError(e.message); }
  }

  return (
    <aside className="panel">
      <div className="panel-header">
        <h3>Licenses — {provider.providerName}</h3>
        <button onClick={onClose}>✕</button>
      </div>
      <p className="muted">
        Provider status: <span className={`badge ${provider.status.toLowerCase()}`}>{provider.status}</span>
        &nbsp;· License validity is determined by expiration date, not provider status.
      </p>

      <ErrorMessage error={error} onRetry={load} />
      {loading && <Loading />}
      {!loading && !error && licenses.length === 0 && <Empty>This provider has no licenses.</Empty>}

      {!loading && licenses.length > 0 && (
        <table>
          <thead><tr><th>Number</th><th>Status</th><th>Expires</th><th>Validity</th></tr></thead>
          <tbody>
            {licenses.map((l) => (
              <tr key={l.licenseId}>
                <td>{l.licenseNumber}</td>
                <td>{l.licenseStatus}</td>
                <td>{l.expirationDate}</td>
                <td>
                  {l.isExpired
                    ? <span className="badge expired">Expired {Math.abs(l.daysUntilExpiration)}d ago</span>
                    : <span className="badge active">Valid · {l.daysUntilExpiration}d left</span>}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}

      <form className="inline-form" onSubmit={addLicense}>
        <input placeholder="License #" value={form.licenseNumber}
               onChange={(e) => setForm({ ...form, licenseNumber: e.target.value })} />
        <select value={form.licenseStatus} onChange={(e) => setForm({ ...form, licenseStatus: e.target.value })}>
          <option>Active</option><option>Expired</option><option>Suspended</option>
        </select>
        <input type="date" value={form.expirationDate}
               onChange={(e) => setForm({ ...form, expirationDate: e.target.value })} />
        <button type="submit" className="primary">Add license</button>
      </form>
      {formError && <div className="alert error">{formError}</div>}
    </aside>
  );
}
