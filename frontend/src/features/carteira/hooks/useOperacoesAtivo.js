import { useAdicionarOperacaoMutation, useRemoverOperacaoMutation } from '../mutations/carteiraMutations'
import { useOperacoesAtivoQuery } from '../queries/carteiraQueries'
import { useCarteiraStore } from '../stores/carteiraStore'

export function useOperacoesAtivo(mostrarToast, solicitarConfirmacao) {
  const ativoSelecionado = useCarteiraStore((state) => state.ativoOperacoesSelecionado)
  const formulario = useCarteiraStore((state) => state.formularioOperacao)
  const abrirOperacoesAtivo = useCarteiraStore((state) => state.abrirOperacoesAtivo)
  const fecharOperacoesAtivo = useCarteiraStore((state) => state.fecharOperacoesAtivo)
  const setCampoFormularioOperacao = useCarteiraStore((state) => state.setCampoFormularioOperacao)
  const resetFormularioOperacao = useCarteiraStore((state) => state.resetFormularioOperacao)
  const operacoesQuery = useOperacoesAtivoQuery(ativoSelecionado?.id)
  const adicionarOperacaoMutation = useAdicionarOperacaoMutation(ativoSelecionado?.id)
  const removerOperacaoMutation = useRemoverOperacaoMutation(ativoSelecionado?.id)

  function abrirPainel(ativo) {
    abrirOperacoesAtivo(ativo)
  }

  function fecharPainel() {
    fecharOperacoesAtivo()
  }

  function atualizarCampo(campo) {
    return (evento) => {
      setCampoFormularioOperacao(campo, evento.target.value)
    }
  }

  async function adicionarOperacao(evento) {
    evento.preventDefault()
    if (!ativoSelecionado) return

    try {
      await adicionarOperacaoMutation.mutateAsync({
        tipo: Number(formulario.tipo),
        data: formulario.data,
        quantidade: Number(formulario.quantidade),
        precoUnitario: Number(formulario.precoUnitario),
        taxas: Number(formulario.taxas || 0),
        observacao: formulario.observacao || null,
      })

      resetFormularioOperacao()
      mostrarToast?.('Operação registrada.', 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao registrar operação', 'erro')
    }
  }

  async function removerOperacao(operacao) {
    const confirmou = await solicitarConfirmacao?.({
      titulo: 'Remover operação',
      mensagem: 'Deseja remover esta operação? A posição do ativo será recalculada.',
      textoConfirmar: 'Remover',
      textoCancelar: 'Cancelar',
      variante: 'perigo',
    })

    if (!confirmou) return

    try {
      await removerOperacaoMutation.mutateAsync(operacao.id)
      mostrarToast?.('Operação removida.', 'sucesso')
    } catch (err) {
      mostrarToast?.(err.message || 'Erro ao remover operação', 'erro')
    }
  }

  return {
    ativoSelecionado,
    operacoes: operacoesQuery.data ?? [],
    formulario,
    carregando: operacoesQuery.isLoading,
    salvando: adicionarOperacaoMutation.isPending,
    abrirPainel,
    fecharPainel,
    atualizarCampo,
    adicionarOperacao,
    removerOperacao,
  }
}
