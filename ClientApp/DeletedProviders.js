import { useCallback, useEffect, useState } from 'react';
import { api } from '../api/client.js';
import { Loading, ErrorMessage, Empty } from './StatusMessage.jsx';

export default function DeletedProviders() {
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const load = useCallback(async () => {
    setLoading(true); setError(null);
    try { setRows(await api.providers.deleted()); }
    catch (e) { setError(e.message); }
    finally { setLoading(false); }
  }, []);

  useEffect(() => { load(); }, [load]);

  async function restore(id) {
    try { await api.providers.restore(id); load(); }
    catch (e) { setError(e.message); }
  }

  return (
    <section>
      <p className="muted">
        Audit view: records still exist in the database but are excluded from all standard queries.
      </p>
      <ErrorMessage error={error} onRetry={load} />
      {loading && <Loading />}
      {!loading && !error && rows.length === 0 && <Empty>No soft-deleted providers.</Empty>}
      {!loading && rows.length > 0 && (
        <table>
          <thead><tr><th>Name</th><th>County</th><th>Status</th><th>Deleted</th><th>By</th><th>Licenses</th><th></th></tr></thead>
          <tbody>
            {rows.map((p) => (
              <tr key={p.providerId}>
                <td>{p.providerName}</td><td>{p.county}</td><td>{p.status}</td>
                <td>{new Date(p.deletedDate).toLocaleString()}</td><td>{p.deletedBy}</td>
                <td>{p.licenses.length}</td>
                <td><button onClick={() => restore(p.providerId)}>Restore</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  );
}
