import { useSelector, useDispatch } from 'react-redux';
import { winRecorded, winRemoved } from './athletesSlice';

function AthleteCard({ name }) {
  const wins = useSelector(state =>
    state.athletes.find(athlete => athlete.name === name).wins
  );
  const dispatch = useDispatch();

  return (
    <div className="athlete-card">
      <h3>{name}</h3>
      <p className="wins-count">{wins}</p>
      <p className="wins-label">wins</p>
      <div className="card-actions">
        <button className="btn btn-primary" onClick={() => dispatch(winRecorded(name))}>+ Win</button>
        <button className="btn btn-ghost" onClick={() => dispatch(winRemoved(name))}>− Win</button>
      </div>
    </div>
  );
}

export default AthleteCard;