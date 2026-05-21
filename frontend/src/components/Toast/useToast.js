import { useCallback, useRef, useState } from 'react'

const DURACAO_PADRAO_MS = 5000
const DURACAO_SAIDA_MS = 280

export function useToast() {
  const [toasts, setToasts] = useState([])
  const proximoId = useRef(0)
  const timeoutsRef = useRef(new Map())

  const removerToast = useCallback((id) => {
    const timeoutExistente = timeoutsRef.current.get(id)
    if (timeoutExistente) {
      clearTimeout(timeoutExistente)
      timeoutsRef.current.delete(id)
    }

    setToasts((atual) => atual.map((toast) => (toast.id === id ? { ...toast, saindo: true } : toast)))

    const timeoutSaida = setTimeout(() => {
      setToasts((atual) => atual.filter((toast) => toast.id !== id))
      timeoutsRef.current.delete(id)
    }, DURACAO_SAIDA_MS)

    timeoutsRef.current.set(`saida-${id}`, timeoutSaida)
  }, [])

  const mostrarToast = useCallback(
    (mensagem, tipo = 'erro', duracaoMs = DURACAO_PADRAO_MS) => {
      const id = ++proximoId.current

      setToasts((atual) => [...atual, { id, mensagem, tipo, saindo: false }])

      if (duracaoMs > 0) {
        const timeoutAuto = setTimeout(() => removerToast(id), duracaoMs)
        timeoutsRef.current.set(id, timeoutAuto)
      }
    },
    [removerToast],
  )

  return { toasts, mostrarToast, removerToast }
}
