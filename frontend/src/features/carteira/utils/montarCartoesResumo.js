import { CONFIG_CARTOES_RESUMO } from '../constants/carteiraSummary.config'

export function montarCartoesResumo(resumo) {
  if (!resumo) return []

  return CONFIG_CARTOES_RESUMO.map(({ rotulo, chave, formatar, comSinal }) => {
    const valorNumerico = resumo[chave]
    const positivo = !comSinal || valorNumerico >= 0

    return {
      id: chave,
      rotulo,
      valorFormatado: formatar(valorNumerico),
      comSinal: Boolean(comSinal),
      positivo,
    }
  })
}
