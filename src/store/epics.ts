import { combineEpics, Epic } from 'redux-observable';
import { from, of } from 'rxjs';
import { map, mergeMap, catchError, filter } from 'rxjs/operators';
import { jobSchedulerApi } from '../api/jobSchedulerApi';
import { RootState } from './index';
import { AppDispatch } from './index';

// Action Types
export const FETCH_JOBS = 'jobs/fetchJobs';
export const FETCH_JOBS_SUCCESS = 'jobs/fetchJobsSuccess';
export const FETCH_JOBS_FAILURE = 'jobs/fetchJobsFailure';

export const RUN_JOB = 'jobs/runJob';
export const RUN_JOB_SUCCESS = 'jobs/runJobSuccess';
export const RUN_JOB_FAILURE = 'jobs/runJobFailure';

export const PAUSE_JOB = 'jobs/pauseJob';
export const PAUSE_JOB_SUCCESS = 'jobs/pauseJobSuccess';
export const PAUSE_JOB_FAILURE = 'jobs/pauseJobFailure';

export const RESUME_JOB = 'jobs/resumeJob';
export const RESUME_JOB_SUCCESS = 'jobs/resumeJobSuccess';
export const RESUME_JOB_FAILURE = 'jobs/resumeJobFailure';

export const DEREGISTER_JOB = 'jobs/deregisterJob';
export const DEREGISTER_JOB_SUCCESS = 'jobs/deregisterJobSuccess';
export const DEREGISTER_JOB_FAILURE = 'jobs/deregisterJobFailure';

// Action Creators
export const fetchJobs = () => ({ type: FETCH_JOBS });
export const runJob = (jobName: string) => ({ type: RUN_JOB, payload: jobName });
export const pauseJob = (jobName: string) => ({ type: PAUSE_JOB, payload: jobName });
export const resumeJob = (jobName: string) => ({ type: RESUME_JOB, payload: jobName });
export const deregisterJob = (jobName: string) => ({ type: DEREGISTER_JOB, payload: jobName });

// Epics
const fetchJobsEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === FETCH_JOBS),
    mergeMap(() =>
      from(jobSchedulerApi.getScheduledJobs()).pipe(
        map((jobs) => ({ type: FETCH_JOBS_SUCCESS, payload: jobs })),
        catchError((error) => of({ type: FETCH_JOBS_FAILURE, payload: error.message }))
      )
    )
  );

const runJobEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === RUN_JOB),
    mergeMap((action) =>
      from(jobSchedulerApi.runJob(action.payload)).pipe(
        map(() => ({ type: RUN_JOB_SUCCESS, payload: action.payload })),
        catchError((error) => of({ type: RUN_JOB_FAILURE, payload: error.message }))
      )
    )
  );

const pauseJobEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === PAUSE_JOB),
    mergeMap((action) =>
      from(jobSchedulerApi.pauseJob(action.payload)).pipe(
        map(() => ({ type: PAUSE_JOB_SUCCESS, payload: action.payload })),
        catchError((error) => of({ type: PAUSE_JOB_FAILURE, payload: error.message }))
      )
    )
  );

const resumeJobEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === RESUME_JOB),
    mergeMap((action) =>
      from(jobSchedulerApi.resumeJob(action.payload)).pipe(
        map(() => ({ type: RESUME_JOB_SUCCESS, payload: action.payload })),
        catchError((error) => of({ type: RESUME_JOB_FAILURE, payload: error.message }))
      )
    )
  );

const deregisterJobEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === DEREGISTER_JOB),
    mergeMap((action) =>
      from(jobSchedulerApi.deregisterJob(action.payload)).pipe(
        map(() => ({ type: DEREGISTER_JOB_SUCCESS, payload: action.payload })),
        catchError((error) => of({ type: DEREGISTER_JOB_FAILURE, payload: error.message }))
      )
    )
  );

// Stats Epics
export const FETCH_STATS = 'stats/fetchStats';
export const FETCH_STATS_SUCCESS = 'stats/fetchStatsSuccess';
export const FETCH_STATS_FAILURE = 'stats/fetchStatsFailure';

export const fetchStats = () => ({ type: FETCH_STATS });

const fetchStatsEpic: Epic = (action$) =>
  action$.pipe(
    filter((action) => action.type === FETCH_STATS),
    mergeMap(() =>
      from(jobSchedulerApi.getJobStats()).pipe(
        map((stats) => ({ type: FETCH_STATS_SUCCESS, payload: stats })),
        catchError((error) => of({ type: FETCH_STATS_FAILURE, payload: error.message }))
      )
    )
  );

// Combine all epics
export const rootEpic = combineEpics(
  fetchJobsEpic,
  runJobEpic,
  pauseJobEpic,
  resumeJobEpic,
  deregisterJobEpic,
  fetchStatsEpic
); 