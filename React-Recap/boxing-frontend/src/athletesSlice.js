import { createSlice } from '@reduxjs/toolkit';

const athletesSlice = createSlice({
    name: 'athletes',
    initialState: [
        { name: 'Canelo', wins: 0 },
        { name: 'Gervonta', wins: 0 },
        { name: 'Tyson', wins: 0 },
        { name: 'Fury', wins: 0 },
    ],
    reducers: {
        winRecorded: (state, action) => {
            const athlete = state.find(athlete => athlete.name === action.payload);
            athlete.wins += 1;
        },
        winRemoved: (state, action) => {
            const athlete = state.find(athlete => athlete.name === action.payload);
            athlete.wins -= 1;
        }
    },
});

export const {winRecorded, winRemoved} = athletesSlice.actions;
export default athletesSlice.reducer;