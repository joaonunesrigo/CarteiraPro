import { cn } from '../../utils/cn'
import { VARIANTES_TOAST } from './toast.styles'

export function Toast({ mensagem, tipo, saindo, onFechar }) {
  return (
    <div
      role="alert"
      className={cn(
        'pointer-events-auto flex w-full max-w-sm items-start gap-3 rounded-lg border px-4 py-3 shadow-lg backdrop-blur-sm',
        VARIANTES_TOAST[tipo] ?? VARIANTES_TOAST.info,
        saindo ? 'animate-toast-out' : 'animate-toast-in',
      )}
    >
      <p className="flex-1 text-sm leading-snug">{mensagem}</p>
      <button
        type="button"
        onClick={onFechar}
        className="shrink-0 text-current opacity-60 transition hover:opacity-100"
        aria-label="Fechar"
      >
        ✕
      </button>
    </div>
  )
}
