import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { montarDadosGraficos } from '../utils/montarDadosGraficos'
import { carregarCarteira } from '../services/carregarCarteira'
import { formatarLinhasAtivos } from '../utils/formatarLinhasAtivos'
import { montarCartoesResumo } from '../utils/montarCartoesResumo'
import { montarResumoCarteira } from '../utils/montarResumoCarteira'
import { normalizarListaRentabilidade } from '../utils/normalizarRentabilidade'

function requisicaoCancelada(erro) {
  return erro?.name === 'AbortError'
}

export function useCarteira() {
  const [resumo, setResumo] = useState(null)
  const [linhasAtivos, setLinhasAtivos] = useState([])
  const [carregando, setCarregando] = useState(true)
  const [erro, setErro] = useState(null)
  const versaoCarregamento = useRef(0)

  const recarregar = useCallback(async () => {
    const versao = ++versaoCarregamento.current
    setCarregando(true)
    setErro(null)

    try {
      const dados = normalizarListaRentabilidade(await carregarCarteira())

      if (versao !== versaoCarregamento.current) return

      setResumo(montarResumoCarteira(dados))
      setLinhasAtivos(formatarLinhasAtivos(dados))
    } catch (e) {
      if (requisicaoCancelada(e)) return
      if (versao !== versaoCarregamento.current) return
      setErro(e.message || 'Erro ao carregar dados')
    } finally {
      if (versao === versaoCarregamento.current) {
        setCarregando(false)
      }
    }
  }, [])

  useEffect(() => {
    recarregar()
    return () => {
      versaoCarregamento.current += 1
    }
  }, [recarregar])

  const cartoesResumo = montarCartoesResumo(resumo)
  const dadosGraficos = useMemo(
    () => montarDadosGraficos(linhasAtivos),
    [linhasAtivos],
  )

  return {
    resumo,
    cartoesResumo,
    linhasAtivos,
    dadosGraficos,
    carregando,
    erro,
    recarregar,
  }
}
