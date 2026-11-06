import { configureStore } from '@reduxjs/toolkit';
import athletesReducer from './athletesSlice';
import { apiSlice } from './apiSlice';

export const store = configureStore({
  reducer: {
    athletes: athletesReducer,
    [apiSlice.reducerPath]: apiSlice.reducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(apiSlice.middleware),
});