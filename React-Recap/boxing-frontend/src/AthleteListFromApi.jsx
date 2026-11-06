import { useState } from 'react';
import { useGetAthletesQuery, useDeleteAthleteMutation, useUpdateAthleteMutation } from './apiSlice';

function AthleteListFromApi() {
  const { data: athletes, isLoading, error } = useGetAthletesQuery();
  const [deleteAthlete] = useDeleteAthleteMutation();
  const [updateAthlete] = useUpdateAthleteMutation();
  const [editingId, setEditingId] = useState(null);
  const [editWins, setEditWins] = useState(0);

  if (isLoading) return <p className="status-msg">Loading...</p>;
  if (error) return <p className="status-msg error">Something went wrong.</p>;

  const startEditing = (athlete) => {
    setEditingId(athlete.id);
    setEditWins(athlete.wins);
  };

  const saveEdit = async (athlete) => {
    await updateAthlete({
      id: athlete.id,
      name: athlete.name,
      weightClass: athlete.weightClass,
      wins: editWins,
      losses: athlete.losses,
    });
    setEditingId(null);
  };

  return (
    <div className="card-grid">
      {athletes.map(athlete => (
        <div className="athlete-card" key={athlete.id}>
          <h3>{athlete.name}</h3>

          {editingId === athlete.id ? (
            <>
              <input
                type="number"
                value={editWins}
                onChange={(e) => setEditWins(Number(e.target.value))}
                className="edit-input"
              />
              <button className="btn btn-primary" onClick={() => saveEdit(athlete)}>Save</button>
            </>
          ) : (
            <>
              <p className="wins-count">{athlete.wins}</p>
              <p className="wins-label">wins</p>
            </>
          )}

          <p className="weight-class">{athlete.weightClass}</p>
          <div className="card-actions">
            <button className="btn btn-ghost" onClick={() => startEditing(athlete)}>Edit</button>
            <button className="btn btn-ghost" onClick={() => deleteAthlete(athlete.id)}>Delete</button>
          </div>
        </div>
      ))}
    </div>
  );
}

export default AthleteListFromApi;