export function SignedValue({ positivo, children, className = 'px-4 py-3 tabular-nums' }) {
  return (
    <td
      className={`${className} ${positivo ? 'text-emerald-400' : 'text-red-400'}`}
    >
      {children}
    </td>
  )
}
