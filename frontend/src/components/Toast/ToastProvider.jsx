import { createContext, useContext } from 'react'
import { ToastContainer } from './ToastContainer'
import { useToast } from './useToast'

const ToastContext = createContext(null)

export function ToastProvider({ children }) {
  const { toasts, mostrarToast, removerToast } = useToast()

  return (
    <ToastContext.Provider value={{ mostrarToast }}>
      {children}
      <ToastContainer toasts={toasts} onFechar={removerToast} />
    </ToastContext.Provider>
  )
}

export function useToastContext() {
  const contexto = useContext(ToastContext)

  if (!contexto) {
    throw new Error('useToastContext deve ser usado dentro de ToastProvider')
  }

  return contexto
}
