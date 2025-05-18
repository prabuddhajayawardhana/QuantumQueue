import { createSlice } from '@reduxjs/toolkit';
import { ScheduledJob } from '../../api/jobSchedulerApi';
import {
  FETCH_JOBS_SUCCESS,
  FETCH_JOBS_FAILURE,
  RUN_JOB_SUCCESS,
  PAUSE_JOB_SUCCESS,
  RESUME_JOB_SUCCESS,
  DEREGISTER_JOB_SUCCESS,
} from '../epics';

interface JobsState {
  items: ScheduledJob[];
  loading: boolean;
  error: string | null;
}

const initialState: JobsState = {
  items: [],
  loading: false,
  error: null,
};

const jobsSlice = createSlice({
  name: 'jobs',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      // Fetch jobs
      .addCase(FETCH_JOBS_SUCCESS, (state, action) => {
        state.loading = false;
        state.items = action.payload;
        state.error = null;
      })
      .addCase(FETCH_JOBS_FAILURE, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      })
      // Run job
      .addCase(RUN_JOB_SUCCESS, (state, action) => {
        const job = state.items.find(j => j.name === action.payload);
        if (job) {
          job.status = 'running';
        }
      })
      // Pause job
      .addCase(PAUSE_JOB_SUCCESS, (state, action) => {
        const job = state.items.find(j => j.name === action.payload);
        if (job) {
          job.status = 'pending';
        }
      })
      // Resume job
      .addCase(RESUME_JOB_SUCCESS, (state, action) => {
        const job = state.items.find(j => j.name === action.payload);
        if (job) {
          job.status = 'running';
        }
      })
      // Deregister job
      .addCase(DEREGISTER_JOB_SUCCESS, (state, action) => {
        state.items = state.items.filter(job => job.name !== action.payload);
      });
  },
});

export default jobsSlice.reducer; 