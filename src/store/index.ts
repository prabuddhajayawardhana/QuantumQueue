import { configureStore } from '@reduxjs/toolkit';
import { createEpicMiddleware } from 'redux-observable';
import { rootEpic } from './epics';
import jobsReducer from './slices/jobsSlice';
import statsReducer from './slices/statsSlice';

const epicMiddleware = createEpicMiddleware();

export const store = configureStore({
  reducer: {
    jobs: jobsReducer,
    stats: statsReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(epicMiddleware),
});

epicMiddleware.run(rootEpic);

export type RootState = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch; 