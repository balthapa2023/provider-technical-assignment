export function Loading() {
  return <p className="muted">Loading…</p>;
}
export function ErrorMessage({ error, onRetry }) {
  if (!error) return null;
  return (
    <div className="alert error">
      {error} {onRetry && <button onClick={onRetry}>Retry</button>}
    </div>
  );
}
export function Empty({ children = 'Nothing to show.' }) {
  return <p className="muted empty">{children}</p>;
}
