import React from 'react';
import { Provider } from 'react-redux';
import Sidebar from './components/Sidebar';
import JobList from './components/JobList';
import JobStats from './components/JobStats';
import { store } from './store';

function App() {
  return (
    <Provider store={store}>
      <div className="flex h-screen bg-gray-100">
        <Sidebar />
        <main className="flex-1 overflow-auto">
          <div className="container mx-auto px-6 py-8">
            <h1 className="text-3xl font-semibold text-gray-800">Job Scheduler Dashboard</h1>
            <JobStats />
            <JobList />
          </div>
        </main>
      </div>
    </Provider>
  );
}

export default App; 