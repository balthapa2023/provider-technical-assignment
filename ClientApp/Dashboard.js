import { useEffect, useState } from 'react';
import { BarChart, Bar, PieChart, Pie, Cell, XAxis, YAxis, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { api } from '../api/client.js';
import { Loading, ErrorMessage } from './StatusMessage.jsx';

const COLORS = ['#2563eb', '#16a34a', '#f59e0b', '#dc2626', '#7c3aed'];
const toChart = (pairs) => pairs.map(({ key, value }) => ({ name: key, value }));

export default function Dashboard() {
  const [data, setData] = useState(null);
  const [error, setError] = useState(null);

  useEffect(() => {
    api.reports.dashboard().then(setData).catch((e) => setError(e.message));
  }, []);

  if (error) return <ErrorMessage error={error} />;
  if (!data) return <Loading />;

  const cards = [
    ['Providers (visible)', data.totalProviders],
    ['Active providers', data.activeProviders],
    ['Soft-deleted providers', data.deletedProviders],
    ['Licenses', data.totalLicenses],
    ['Expired licenses', data.expiredLicenses],
    ['Expiring in 30 days', data.expiringIn30Days]
  ];

  return (
    <section>
      <div className="cards">
        {cards.map(([label, value]) => (
          <div className="card" key={label}><div className="card-value">{value}</div><div className="muted">{label}</div></div>
        ))}
      </div>

      <div className="charts">
        <div className="chart">
          <h4>Providers by status</h4>
          <ResponsiveContainer width="100%" height={240}>
            <PieChart>
              <Pie data={toChart(data.providersByStatus)} dataKey="value" nameKey="name" label>
                {data.providersByStatus.map((_, i) => <Cell key={i} fill={COLORS[i % COLORS.length]} />)}
              </Pie>
              <Tooltip /><Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="chart">
          <h4>Active vs expired licenses</h4>
          <ResponsiveContainer width="100%" height={240}>
            <PieChart>
              <Pie data={[{ name: 'Active', value: data.activeLicenses }, { name: 'Expired', value: data.expiredLicenses }]}
                   dataKey="value" nameKey="name" label>
                <Cell fill="#16a34a" /><Cell fill="#dc2626" />
              </Pie>
              <Tooltip /><Legend />
            </PieChart>
          </ResponsiveContainer>
        </div>

        <div className="chart wide">
          <h4>Licenses per provider</h4>
          <ResponsiveContainer width="100%" height={260}>
            <BarChart data={toChart(data.licensesPerProvider)}>
              <XAxis dataKey="name" interval={0} angle={-15} textAnchor="end" height={60} />
              <YAxis allowDecimals={false} /><Tooltip />
              <Bar dataKey="value" fill="#2563eb" />
            </BarChart>
          </ResponsiveContainer>
        </div>
      </div>
    </section>
  );
}
