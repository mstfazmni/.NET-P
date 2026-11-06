import { useSelector } from 'react-redux';
import AthleteCard from './AthleteCard';
import AthleteListFromApi from './AthleteListFromApi';
import CreateAthleteForm from './CreateAthleteForm';
import './App.css';

function App() {
  const athletes = useSelector(state => state.athletes);
  const totalWins = athletes.reduce((total, athlete) => total + athlete.wins, 0);

  return (
    <div className="app">
      <header className="hero">
        <h1>BOXING<span className="accent">APP</span></h1>
        <p className="tagline">Track fighters. Manage records. Own the ring.</p>
        <div className="stat-pill">Total wins tracked: <strong>{totalWins}</strong></div>
      </header>

      <main>
        <section>
          <h2 className="section-title">Local Roster</h2>
          <div className="card-grid">
            {athletes.map(athlete => (
              <AthleteCard key={athlete.name} name={athlete.name} />
            ))}
          </div>
        </section>

        <section>
          <h2 className="section-title">Live From API</h2>
          <AthleteListFromApi />
          <CreateAthleteForm />
        </section>
      </main>
    </div>
  );
}

export default App;