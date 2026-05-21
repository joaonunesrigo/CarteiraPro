import { carteiraApi } from '../services/carteira.api'
import { useAdicionarAtivoMutation, useRemoverAtivoMutation, useRemoverTodosAtivosMutation } from '../mutations/carteiraMutations'
import { useCarteiraStore } from '../stores/carteiraStore'
import { formularioTemErros, validarFormularioAtivo } from '../utils/validarFormularioAtivo'
import { normalizarTicker } from '../utils/validarTicker'

export function useAdicionarAtivo(tickersCadastrados = [], mostrarToast) {
  const formulario = useCarteiraStore((state) => state.formularioAtivo)
  const errosCampos = useCarteiraStore((state) => state.errosAtivo)
  const enviando = useCarteiraStore((state) => state.enviandoAtivo)
  const setCampoFormularioAtivo = useCarteiraStore((state) => state.setCampoFormularioAtivo)
  const setErrosAtivo = useCarteiraStore((state) => state.setErrosAtivo)
  const setEnviandoAtivo = useCarteiraStore((state) => state.setEnviandoAtivo)
  const resetFormularioAtivo = useCarteiraStore((state) => state.resetFormularioAtivo)
  const adicionarAtivo = useAdicionarAtivoMutation()

  function atualizarCampo(campo) {
    return (evento) => {
      setCampoFormularioAtivo(campo, evento.target.value)
    }
  }

  async function validarTickerExterno(ticker) {
    try {
      await carteiraApi.obterCotacao(ticker)
      return null
    } catch {
      return `O ticker ${ticker} não foi encontrado. Verifique o símbolo e tente novamente.`
    }
  }

  async function enviarFormulario(evento) {
    evento.preventDefault()
    setEnviandoAtivo(true)

    const erros = validarFormularioAtivo(formulario, tickersCadastrados)

    if (formularioTemErros(erros)) {
      setErrosAtivo(erros)
      setEnviandoAtivo(false)
      return
    }

    const ticker = normalizarTicker(formulario.ticker)
    const erroBrapi = await validarTickerExterno(ticker)

    if (erroBrapi) {
      setErrosAtivo({ ticker: erroBrapi })
      setEnviandoAtivo(false)
      return
    }

    try {
      await adicionarAtivo.mutateAsync({
        ticker,
        precoMedio: Number(formulario.precoMedio),
        quantidade: Number(formulario.quantidade),
        tipo: Number(formulario.tipo),
      })
      resetFormularioAtivo()
      mostrarToast(`${ticker} adicionado à carteira.`, 'sucesso')
    } catch (err) {
      const mensagem = err.message || 'Erro ao adicionar ativo'
      if (
        mensagem.toLowerCase().includes('ticker') ||
        mensagem.toLowerCase().includes('encontrado') ||
        mensagem.toLowerCase().includes('cadastrado')
      ) {
        setErrosAtivo({ ticker: mensagem })
      } else {
        mostrarToast(mensagem, 'erro')
      }
    } finally {
      setEnviandoAtivo(false)
    }
  }

  return {
    formulario,
    errosCampos,
    atualizarCampo,
    enviando,
    enviarFormulario,
  }
}

export function useExcluirAtivo(mostrarToast, solicitarConfirmacao) {
  const removerAtivo = useRemoverAtivoMutation()
  const removerTodosAtivos = useRemoverTodosAtivosMutation()

  async function excluirAtivo(id, ticker) {
    if (!id) return

    const confirmou = await solicitarConfirmacao({
      titulo: 'Remover ativo',
      mensagem: `Deseja remover ${ticker} da carteira? Esta ação não pode ser desfeita.`,
      textoConfirmar: 'Remover',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      await removerAtivo.mutateAsync(id)
      mostrarToast?.(`${ticker} foi removido da carteira.`, 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover ativo', 'erro')
    }
  }

  async function excluirTodosAtivos(quantidade) {
    if (!quantidade) return

    const confirmou = await solicitarConfirmacao({
      titulo: 'Remover todos os ativos',
      mensagem: `Deseja remover os ${quantidade} ativos da carteira? Esta ação não pode ser desfeita.`,
      textoConfirmar: 'Remover todos',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      const resultado = await removerTodosAtivos.mutateAsync()
      const removidos = resultado?.removidos ?? quantidade
      mostrarToast?.(`${removidos} ativo(s) removido(s) da carteira.`, 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover ativos', 'erro')
    }
  }

  return { excluirAtivo, excluirTodosAtivos }
}
