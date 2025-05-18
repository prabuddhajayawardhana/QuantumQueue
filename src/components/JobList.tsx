import React, { useEffect } from 'react';
import { useDispatch, useSelector } from 'react-redux';
import { format } from 'date-fns';
import { RootState } from '../store';
import { fetchJobs, runJob, pauseJob, resumeJob, deregisterJob } from '../store/slices/jobsSlice';
import { PlayIcon, PauseIcon, PlayPauseIcon, TrashIcon } from '@heroicons/react/24/outline';

const JobList = () => {
  const dispatch = useDispatch();
  const { items: jobs, loading, error } = useSelector((state: RootState) => state.jobs);

  useEffect(() => {
    dispatch(fetchJobs());
  }, [dispatch]);

  const handleRunJob = (jobName: string) => {
    dispatch(runJob(jobName));
  };

  const handlePauseJob = (jobName: string) => {
    dispatch(pauseJob(jobName));
  };

  const handleResumeJob = (jobName: string) => {
    dispatch(resumeJob(jobName));
  };

  const handleDeregisterJob = (jobName: string) => {
    dispatch(deregisterJob(jobName));
  };

  if (loading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-500"></div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="mt-8 bg-red-50 border border-red-200 rounded-lg p-4">
        <p className="text-red-700">{error}</p>
      </div>
    );
  }

  return (
    <div className="mt-8">
      <div className="bg-white shadow-sm rounded-lg">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-lg font-medium text-gray-900 mb-4">Scheduled Jobs</h3>
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead>
                <tr>
                  <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Name
                  </th>
                  <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Status
                  </th>
                  <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Next Run
                  </th>
                  <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Last Run
                  </th>
                  <th className="px-6 py-3 bg-gray-50 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {jobs.map((job) => (
                  <tr key={job.id}>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {job.name}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm">
                      <span
                        className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${
                          job.status === 'completed'
                            ? 'bg-green-100 text-green-800'
                            : job.status === 'running'
                            ? 'bg-blue-100 text-blue-800'
                            : job.status === 'failed'
                            ? 'bg-red-100 text-red-800'
                            : 'bg-yellow-100 text-yellow-800'
                        }`}
                      >
                        {job.status}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {format(new Date(job.nextRun), 'PPp')}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {job.lastRun ? format(new Date(job.lastRun), 'PPp') : '-'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      <div className="flex space-x-2">
                        {job.status === 'pending' ? (
                          <button
                            onClick={() => handleRunJob(job.name)}
                            className="text-blue-600 hover:text-blue-900"
                          >
                            <PlayIcon className="h-5 w-5" />
                          </button>
                        ) : job.status === 'running' ? (
                          <button
                            onClick={() => handlePauseJob(job.name)}
                            className="text-yellow-600 hover:text-yellow-900"
                          >
                            <PauseIcon className="h-5 w-5" />
                          </button>
                        ) : (
                          <button
                            onClick={() => handleResumeJob(job.name)}
                            className="text-green-600 hover:text-green-900"
                          >
                            <PlayPauseIcon className="h-5 w-5" />
                          </button>
                        )}
                        <button
                          onClick={() => handleDeregisterJob(job.name)}
                          className="text-red-600 hover:text-red-900"
                        >
                          <TrashIcon className="h-5 w-5" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default JobList; 