import { useState } from 'react';
import ProviderList from './components/ProviderList.jsx';
import ScenariosView from './components/ScenariosView.jsx';
import DeletedProviders from './components/DeletedProviders.jsx';
import Dashboard from './components/Dashboard.jsx';

const TABS = [
  ['providers', 'Providers'],
  ['scenarios', 'License Scenarios'],
  ['deleted', 'Deleted (Audit)'],
  ['dashboard', 'Dashboard']
];

export default function App() {
  const [tab, setTab] = useState('providers');

  return (
    <div className="container">
      <header>
        <h1>Provider &amp; License Management</h1>
        <nav>
          {TABS.map(([key, label]) => (
            <button key={key} className={tab === key ? 'active' : ''} onClick={() => setTab(key)}>
              {label}
            </button>
          ))}
        </nav>
      </header>

      {tab === 'providers' && <ProviderList />}
      {tab === 'scenarios' && <ScenariosView />}
      {tab === 'deleted' && <DeletedProviders />}
      {tab === 'dashboard' && <Dashboard />}
    </div>
  );
}
