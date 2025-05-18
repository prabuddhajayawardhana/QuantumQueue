import React from 'react';
import {
  ClockIcon,
  QueueListIcon,
  ChartBarIcon,
  CogIcon,
} from '@heroicons/react/24/outline';

const navigation = [
  { name: 'Dashboard', icon: ChartBarIcon, href: '#', current: true },
  { name: 'Jobs', icon: QueueListIcon, href: '#', current: false },
  { name: 'Schedule', icon: ClockIcon, href: '#', current: false },
  { name: 'Settings', icon: CogIcon, href: '#', current: false },
];

const Sidebar = () => {
  return (
    <div className="flex flex-col w-64 bg-white border-r">
      <div className="flex items-center justify-center h-16 border-b">
        <span className="text-xl font-semibold text-gray-800">QuantumQueue</span>
      </div>
      <nav className="flex-1 px-2 py-4 space-y-1">
        {navigation.map((item) => (
          <a
            key={item.name}
            href={item.href}
            className={`flex items-center px-4 py-2 text-sm font-medium rounded-md ${
              item.current
                ? 'bg-primary-100 text-primary-700'
                : 'text-gray-600 hover:bg-gray-50'
            }`}
          >
            <item.icon
              className={`mr-3 h-6 w-6 ${
                item.current ? 'text-primary-700' : 'text-gray-400'
              }`}
              aria-hidden="true"
            />
            {item.name}
          </a>
        ))}
      </nav>
    </div>
  );
};

export default Sidebar; 