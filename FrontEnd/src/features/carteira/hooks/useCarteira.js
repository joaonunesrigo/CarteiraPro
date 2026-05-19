import { useCallback, useEffect, useState } from 'react'
import { carteiraApi } from '../services/carteira.api'
import { formatarLinhasAtivos } from '../utils/formatarLinhasAtivos'
import { mesclarAtivosComIds } from '../utils/mesclarAtivosComIds'
import { montarCartoesResumo } from '../utils/montarCartoesResumo'

export function useCarteira() {
  const [resumo, setResumo] = useState(null)
  const [linhasAtivos, setLinhasAtivos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)

  const recarregar = useCallback(async () => {
    setCarregando(true)
    setErro(null)
    try {
      const [resumoData, rentabilidade, ativos] = await Promise.all([
        carteiraApi.obterResumo(),
        carteiraApi.obterRentabilidade(),
        carteiraApi.listarAtivos(),
      ])
      setResumo(resumoData)
      setLinhasAtivos(
        formatarLinhasAtivos(mesclarAtivosComIds(rentabilidade, ativos)),
      )
    } catch (e) {
      setErro(e.message || 'Erro ao carregar dados')
    } finally {
      setCarregando(false)
    }
  }, [])

  useEffect(() => {
    recarregar()
  }, [recarregar])

  const cartoesResumo = montarCartoesResumo(resumo)

  return {
    resumo,
    cartoesResumo,
    linhasAtivos,
    carregando,
    erro,
    recarregar,
  }
}
