import { createContext, useContext } from 'react'
import { ConfirmDialog } from './ConfirmDialog'
import { useConfirmDialog } from './useConfirmDialog'

const ConfirmDialogContext = createContext(null)

export function ConfirmDialogProvider({ children }) {
  const { dialogo, solicitarConfirmacao, confirmar, cancelar } =
    useConfirmDialog()

  return (
    <ConfirmDialogContext.Provider value={{ solicitarConfirmacao }}>
      {children}
      {dialogo && (
        <ConfirmDialog
          {...dialogo}
          onConfirmar={confirmar}
          onCancelar={cancelar}
        />
      )}
    </ConfirmDialogContext.Provider>
  )
}

export function useConfirmDialogContext() {
  const contexto = useContext(ConfirmDialogContext)

  if (!contexto) {
    throw new Error(
      'useConfirmDialogContext deve ser usado dentro de ConfirmDialogProvider',
    )
  }

  return contexto
}
