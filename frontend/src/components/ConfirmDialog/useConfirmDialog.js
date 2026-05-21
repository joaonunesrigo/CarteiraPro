import { useCallback, useEffect, useRef, useState } from 'react'

const OPCOES_PADRAO = {
  titulo: 'Confirmar',
  mensagem: 'Deseja continuar?',
  textoConfirmar: 'Confirmar',
  textoCancelar: 'Cancelar',
  variante: 'primary',
}

export function useConfirmDialog() {
  const [dialogo, setDialogo] = useState(null)
  const resolverRef = useRef(null)

  const fecharDialogo = useCallback((confirmado) => {
    setDialogo(null)
    resolverRef.current?.(confirmado)
    resolverRef.current = null
  }, [])

  const solicitarConfirmacao = useCallback((opcoes = {}) => {
    return new Promise((resolve) => {
      resolverRef.current = resolve
      setDialogo({ ...OPCOES_PADRAO, ...opcoes })
    })
  }, [])

  useEffect(() => {
    if (!dialogo) return

    function aoPressionarTecla(evento) {
      if (evento.key === 'Escape') fecharDialogo(false)
    }

    document.addEventListener('keydown', aoPressionarTecla)
    return () => document.removeEventListener('keydown', aoPressionarTecla)
  }, [dialogo, fecharDialogo])

  return {
    dialogo,
    solicitarConfirmacao,
    confirmar: () => fecharDialogo(true),
    cancelar: () => fecharDialogo(false),
  }
}
