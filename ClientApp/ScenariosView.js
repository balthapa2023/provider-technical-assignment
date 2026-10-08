import { useEffect, useState } from 'react';
import { api } from '../api/client.js';
import { Loading, ErrorMessage, Empty } from './StatusMessage.jsx';

const SCENARIOS = {
  activeActive: { title: 'Active providers with active licenses', fetch: api.reports.activeActive },
  activeExpired: { title: 'Providers that appear Active but have EXPIRED licenses', fetch: api.reports.activeExpired }
};

export default function ScenariosView() {
  const [key, setKey] = useState('activeActive');
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;
    setLoading(true); setError(null);
    SCENARIOS[key].fetch()
      .then((d) => !cancelled && setRows(d))
      .catch((e) => !cancelled && setError(e.message))
      .finally(() => !cancelled && setLoading(false));
    return () => { cancelled = true; };
  }, [key]);

  return (
    <section>
      <div className="toolbar">
        {Object.entries(SCENARIOS).map(([k, s]) => (
          <button key={k} className={key === k ? 'active' : ''} onClick={() => setKey(k)}>{s.title}</button>
        ))}
      </div>
      <p className="muted">Soft-deleted providers are excluded from every scenario by the API.</p>

      <ErrorMessage error={error} />
      {loading && <Loading />}
      {!loading && !error && rows.length === 0 && <Empty>No records for this scenario.</Empty>}

      {!loading && rows.length > 0 && (
        <table>
          <thead><tr><th>Provider</th><th>County</th><th>Provider Status</th><th>License #</th><th>License Status</th><th>Expires</th></tr></thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.licenseId}>
                <td>{r.providerName}</td><td>{r.county}</td>
                <td><span className={`badge ${r.providerStatus.toLowerCase()}`}>{r.providerStatus}</span></td>
                <td>{r.licenseNumber}</td><td>{r.licenseStatus}</td>
                <td className={r.isExpired ? 'text-danger' : ''}>{r.expirationDate}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  );
}
