import { useCallback, useEffect, useState } from 'react'
import { proventosApi } from '../services/proventos.api'

export function useProventos() {
  const [proventos, setProventos] = useState([])
  const [resumo, setResumo] = useState(null)
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)

  const recarregar = useCallback(async () => {
    setCarregando(true)
    setErro(null)

    try {
      const [lista, resumoResposta] = await Promise.all([
        proventosApi.listar(),
        proventosApi.obterResumo(),
      ])

      setProventos(lista ?? [])
      setResumo(resumoResposta ?? null)
    } catch (err) {
      setErro(err.message || 'Erro ao carregar proventos.')
    } finally {
      setCarregando(false)
    }
  }, [])

  useEffect(() => {
    recarregar()
  }, [recarregar])

  return {
    proventos,
    resumo,
    carregando,
    erro,
    recarregar,
  }
}
