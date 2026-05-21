import { useEffect } from 'react'

const TAMANHOS = {
  sm: 'max-w-md',
  md: 'max-w-xl',
  lg: 'max-w-3xl',
  xl: 'max-w-5xl',
  full: 'max-w-7xl',
}

export function Modal({ aberto, titulo, acaoCabecalho, onFechar, tamanho = 'lg', children }) {
  useEffect(() => {
    if (!aberto) return undefined

    function aoPressionarTecla(evento) {
      if (evento.key === 'Escape') onFechar?.()
    }

    document.addEventListener('keydown', aoPressionarTecla)
    return () => document.removeEventListener('keydown', aoPressionarTecla)
  }, [aberto, onFechar])

  useEffect(() => {
    if (!aberto) return undefined

    const overflowAnterior = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => {
      document.body.style.overflow = overflowAnterior
    }
  }, [aberto])

  if (!aberto) return null

  const classeTamanho = TAMANHOS[tamanho] ?? TAMANHOS.lg

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4" role="presentation">
      <button
        type="button"
        className="absolute inset-0 animate-modal-overlay-in bg-slate-950/70 backdrop-blur-sm"
        aria-label="Fechar"
        onClick={onFechar}
      />

      <div
        role="dialog"
        aria-modal="true"
        aria-labelledby="modal-titulo"
        className={`relative flex max-h-[90vh] w-full ${classeTamanho} animate-modal-content-in flex-col overflow-hidden rounded-xl border border-slate-700 bg-slate-900 opacity-0 shadow-xl`}
      >
        <div className="flex items-center justify-between gap-4 border-b border-slate-800 px-5 py-4">
          <h2 id="modal-titulo" className="text-lg font-semibold text-white">
            {titulo}
          </h2>

          <div className="flex items-center gap-2">
            {acaoCabecalho}
            <button
              type="button"
              onClick={onFechar}
              aria-label="Fechar"
              className="rounded-md p-1 text-slate-400 transition hover:bg-slate-800 hover:text-white"
            >
              <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor" className="h-5 w-5">
                <path
                  fillRule="evenodd"
                  d="M4.28 4.22a.75.75 0 011.06 0L10 8.94l4.66-4.72a.75.75 0 111.06 1.06L11.06 10l4.66 4.72a.75.75 0 11-1.06 1.06L10 11.06l-4.66 4.72a.75.75 0 11-1.06-1.06L8.94 10 4.28 5.28a.75.75 0 010-1.06z"
                  clipRule="evenodd"
                />
              </svg>
            </button>
          </div>
        </div>

        <div className="overflow-y-auto px-5 py-4">{children}</div>
      </div>
    </div>
  )
}
