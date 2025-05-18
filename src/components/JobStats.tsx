import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import {
  CheckCircleIcon,
  XCircleIcon,
  ClockIcon,
  PlayCircleIcon,
} from '@heroicons/react/24/outline';
import { RootState } from '../store';
import { fetchStats } from '../store/slices/statsSlice';

const StatCard = ({
  title,
  value,
  icon: Icon,
  color,
}: {
  title: string;
  value: number;
  icon: React.ComponentType<any>;
  color: string;
}) => (
  <div className="bg-white rounded-lg shadow-sm p-6">
    <div className="flex items-center">
      <div className="flex-shrink-0">
        <Icon className={`h-8 w-8 ${color}`} />
      </div>
      <div className="ml-5 w-0 flex-1">
        <dl>
          <dt className="text-sm font-medium text-gray-500 truncate">{title}</dt>
          <dd className="flex items-baseline">
            <div className="text-2xl font-semibold text-gray-900">{value}</div>
          </dd>
        </dl>
      </div>
    </div>
  </div>
);

const JobStats = () => {
  const dispatch = useDispatch();
  const { data: stats, loading, error } = useSelector((state: RootState) => state.stats);

  useEffect(() => {
    dispatch(fetchStats());
  }, [dispatch]);

  if (loading || !stats) {
    return (
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mt-4">
        {[...Array(4)].map((_, i) => (
          <div
            key={i}
            className="bg-white rounded-lg shadow-sm p-6 animate-pulse"
          >
            <div className="h-8 bg-gray-200 rounded w-8"></div>
            <div className="space-y-3 mt-4">
              <div className="h-4 bg-gray-200 rounded w-1/2"></div>
              <div className="h-6 bg-gray-200 rounded w-1/4"></div>
            </div>
          </div>
        ))}
      </div>
    );
  }

  if (error) {
    return (
      <div className="mt-4 bg-red-50 border border-red-200 rounded-lg p-4">
        <p className="text-red-700">{error}</p>
      </div>
    );
  }

  return (
    <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4 mt-4">
      <StatCard
        title="Completed Jobs"
        value={stats.completed}
        icon={CheckCircleIcon}
        color="text-green-500"
      />
      <StatCard
        title="Failed Jobs"
        value={stats.failed}
        icon={XCircleIcon}
        color="text-red-500"
      />
      <StatCard
        title="Pending Jobs"
        value={stats.pending}
        icon={ClockIcon}
        color="text-yellow-500"
      />
      <StatCard
        title="Running Jobs"
        value={stats.running}
        icon={PlayCircleIcon}
        color="text-blue-500"
      />
    </div>
  );
};

export default JobStats;
