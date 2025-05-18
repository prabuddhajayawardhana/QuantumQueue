import axios from 'axios';

const API_BASE_URL = 'http://localhost:5000/api'; // Adjust this to your backend URL

export interface ScheduledJob {
  id: string;
  name: string;
  status: 'pending' | 'running' | 'completed' | 'failed';
  nextRun: string;
  lastRun?: string;
}

export interface JobStats {
  completed: number;
  failed: number;
  pending: number;
  running: number;
}

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const jobSchedulerApi = {
  // Get all scheduled jobs
  getScheduledJobs: async (): Promise<ScheduledJob[]> => {
    const response = await api.get('/jobs');
    return response.data;
  },

  // Run a specific job
  runJob: async (jobName: string): Promise<void> => {
    await api.post(`/jobs/${jobName}/run`);
  },

  // Pause a job
  pauseJob: async (jobName: string): Promise<void> => {
    await api.post(`/jobs/${jobName}/pause`);
  },

  // Resume a job
  resumeJob: async (jobName: string): Promise<void> => {
    await api.post(`/jobs/${jobName}/resume`);
  },

  // Deregister a job
  deregisterJob: async (jobName: string): Promise<void> => {
    await api.delete(`/jobs/${jobName}`);
  },

  // Get next run time for a job
  getNextRunTime: async (jobName: string): Promise<string> => {
    const response = await api.get(`/jobs/${jobName}/next-run`);
    return response.data.nextRunTime;
  },

  // Get job statistics
  getJobStats: async (): Promise<JobStats> => {
    const response = await api.get('/jobs/stats');
    return response.data;
  },
}; 