export function Alert({ children, className = '' }) {
  return (
    <p
      className={`rounded-lg bg-red-500/10 px-4 py-3 text-red-300 ${className}`.trim()}
      role="alert"
    >
      {children}
    </p>
  )
}
