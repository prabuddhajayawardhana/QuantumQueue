import { createSlice } from '@reduxjs/toolkit';
import { JobStats } from '../../api/jobSchedulerApi';
import {
  FETCH_STATS_SUCCESS,
  FETCH_STATS_FAILURE,
} from '../epics';

interface StatsState {
  data: JobStats | null;
  loading: boolean;
  error: string | null;
}

const initialState: StatsState = {
  data: null,
  loading: false,
  error: null,
};

const statsSlice = createSlice({
  name: 'stats',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(FETCH_STATS_SUCCESS, (state, action) => {
        state.loading = false;
        state.data = action.payload;
        state.error = null;
      })
      .addCase(FETCH_STATS_FAILURE, (state, action) => {
        state.loading = false;
        state.error = action.payload;
      });
  },
});

export default statsSlice.reducer; 