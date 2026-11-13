import { useEffect, useState } from 'react'
import { getDashboard } from './api.js'

function BarChart({ title, description, items }) {
  const maximum = Math.max(1, ...items.map((item) => item.count))

  return (
    <section className="dashboard-panel" aria-label={title}>
      <h2>{title}</h2>
      <p className="dashboard-caption">{description}</p>
      {items.length === 0 ? (
        <p className="state-message">No providers to display.</p>
      ) : (
        <ul className="chart-bars">
          {items.map((item) => (
            <li key={item.id}>
              <div className="chart-label">
                <span>{item.label}</span>
                <strong>{item.count}</strong>
              </div>
              <div className="chart-track" aria-hidden="true">
                <div className="chart-fill" style={{ width: `${item.count / maximum * 100}%` }} />
              </div>
            </li>
          ))}
        </ul>
      )}
    </section>
  )
}

export default function Dashboard() {
  const [data, setData] = useState(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState('')
  const [reloadKey, setReloadKey] = useState(0)

  useEffect(() => {
    const controller = new AbortController()
    setLoading(true)
    setError('')
    setData(null)

    async function load() {
      try {
        setData(await getDashboard(controller.signal))
      } catch (requestError) {
        if (requestError.name !== 'AbortError') {
          setError(requestError.message)
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }

    load()
    return () => controller.abort()
  }, [reloadKey])

  return (
    <>
      <section className="page-heading">
        <div>
          <p className="eyebrow">READ-ONLY INSIGHTS</p>
          <h1>Provider &amp; license dashboard</h1>
          <p className="page-description">Registry-wide health and upcoming renewals. Excludes soft-deleted records.</p>
        </div>
        <button
          className="button button-secondary"
          disabled={loading}
          onClick={() => setReloadKey((current) => current + 1)}
        >
          Refresh
        </button>
      </section>

      <div className="dashboard-content" aria-live="polite" aria-busy={loading}>
        {loading && <p className="state-message">Loading dashboard...</p>}
        {error && (
          <div className="notice notice-error" role="alert">
            <strong>Could not load dashboard.</strong> {error}
            <button className="text-button" onClick={() => setReloadKey((current) => current + 1)}>
              Try again
            </button>
          </div>
        )}
        {!loading && data && (
          <>
            <p className="dashboard-caption">
              Evaluated on {data.asOfDate} (UTC). Renewal window: today through {data.expiringWithinDays} days ahead, inclusive.
            </p>
            <section className="dashboard-summary" aria-label="Registry summary">
              {[
                ['Providers', data.providerCount, 'All lifecycle statuses'],
                ['Licenses', data.licenseCount, 'Across current providers'],
                ['Expired licenses', data.expiredLicenseCount, 'Past expiration; not suspended'],
                ['Providers needing renewal', data.providersWithExpiringLicensesCount, `Active licenses expiring within ${data.expiringWithinDays} days`],
              ].map(([label, value, detail]) => (
                <div className="summary-card" key={label}>
                  <span className="summary-label">{label}</span>
                  <strong className="summary-value">{value}</strong>
                  <span className="summary-detail">{detail}</span>
                </div>
              ))}
            </section>
            <div className="dashboard-charts">
              <BarChart
                title="Providers by status"
                description="Lifecycle distribution across the registry."
                items={data.providersByStatus.map((item) => ({ id: item.status, label: item.status, count: item.count }))}
              />
              <BarChart
                title="License health"
                description="Backend-derived validity; suspended licenses remain separate."
                items={data.licensesByStatus.map((item) => ({ id: item.status, label: item.status, count: item.count }))}
              />
              <BarChart
                title="Licenses per provider"
                description="All providers, including those with no licenses; most licenses first."
                items={data.licensesPerProvider.map((item) => ({ id: item.providerId, label: item.providerName, count: item.count }))}
              />
            </div>
            <section className="dashboard-panel">
              <h2>Providers with expiring licenses</h2>
              <p className="dashboard-caption">
                Currently active licenses only, across all provider statuses. Earliest renewal first.
              </p>
              {data.providersWithExpiringLicenses.length === 0 ? (
                <p className="state-message">No providers have active licenses expiring within this window.</p>
              ) : (
                <div className="dashboard-table-scroll">
                  <table className="dashboard-table">
                    <caption className="visually-hidden">Providers with active licenses expiring within {data.expiringWithinDays} days</caption>
                    <thead>
                      <tr><th scope="col">Provider</th><th scope="col">County</th><th scope="col">Expiring licenses</th><th scope="col">Next expiration (UTC)</th></tr>
                    </thead>
                    <tbody>
                      {data.providersWithExpiringLicenses.map((provider) => (
                        <tr key={provider.providerId}>
                          <th scope="row">{provider.providerName}</th>
                          <td>{provider.county}</td>
                          <td>{provider.expiringLicenseCount}</td>
                          <td>{provider.nextExpirationDate}</td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              )}
            </section>
          </>
        )}
      </div>
    </>
  )
}
