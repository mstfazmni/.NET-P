import { useState } from 'react';
import { useCreateAthleteMutation } from './apiSlice';

function CreateAthleteForm() {
  const [name, setName] = useState('');
  const [weightClass, setWeightClass] = useState('');
  const [createAthlete, { isLoading }] = useCreateAthleteMutation();

  const handleSubmit = async (e) => {
    e.preventDefault();
    await createAthlete({ name, weightClass, wins: 0, losses: 0 });
    setName('');
    setWeightClass('');
  };

  return (
    <form onSubmit={handleSubmit} className="create-form">
      <input
        type="text"
        placeholder="Athlete name"
        value={name}
        onChange={(e) => setName(e.target.value)}
        required
      />
      <input
        type="text"
        placeholder="Weight class"
        value={weightClass}
        onChange={(e) => setWeightClass(e.target.value)}
        required
      />
      <button type="submit" className="btn btn-primary" disabled={isLoading}>
        {isLoading ? 'Adding...' : 'Add Athlete'}
      </button>
    </form>
  );
}

export default CreateAthleteForm;