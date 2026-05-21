import { Toast } from './Toast'

export function ToastContainer({ toasts, onFechar }) {
  if (!toasts.length) return null

  return (
    <div aria-live="polite" className="pointer-events-none fixed bottom-4 right-4 z-50 flex w-full max-w-sm flex-col-reverse gap-2">
      {toasts.map((toast) => (
        <Toast key={toast.id} mensagem={toast.mensagem} tipo={toast.tipo} saindo={toast.saindo} onFechar={() => onFechar(toast.id)} />
      ))}
    </div>
  )
}
