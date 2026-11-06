import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const apiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({ baseUrl: 'http://localhost:5213/api' }),
  endpoints: (builder) => ({
    getAthletes: builder.query({
      query: () => '/Athletes',
      providesTags: ['Athlete'],
    }),
    deleteAthlete: builder.mutation({
      query: (id) => ({
        url: `/Athletes/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: ['Athlete'],
    }),
    createAthlete: builder.mutation({
      query: (newAthlete) => ({
        url: 'Athletes',
        method: 'POST',
        body: newAthlete,
      }),
      invalidatesTags: ['Athlete']
    }),
    updateAthlete: builder.mutation({
      query: ({ id, ...updates }) => ({
        url: `Athletes/${id}`,
        method: 'PUT',
        body: updates
      }),
      invalidatesTags: ['Athlete'],
    })
  }),
});

export const { useGetAthletesQuery, useDeleteAthleteMutation, useCreateAthleteMutation, useUpdateAthleteMutation } = apiSlice;

// Breaking this down, piece by piece:
// fetchBaseQuery({ baseUrl: '...' }) — this is just saying 
// "every request from this API slice starts with this base URL."
//  Same idea as setting a default in axios.create({ baseURL: ... }) if you've used axios before.
// getAthletes: builder.query({ query: () => '/Athletes' }) — this defines one specific 
// "way of fetching data." query: () => '/Athletes' means 
// "when this is called, make a GET request to baseUrl + '/Athletes'" — 
// which resolves to http://localhost:5213/api/Athletes, exactly your existing endpoint.
// export const { useGetAthletesQuery } — this is the genuinely magic part. 
// RTK Query automatically generates a React hook for you, named use + your endpoint name + Query. 
// You never manually write this hook — it's created based on the getAthletes key you defined above.