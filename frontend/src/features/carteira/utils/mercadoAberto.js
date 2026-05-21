const HORA_ABERTURA_PREGAO = 10
const HORA_FECHAMENTO_PREGAO = 18

function partesDataSaoPaulo(data = new Date()) {
  const partes = new Intl.DateTimeFormat('pt-BR', {
    timeZone: 'America/Sao_Paulo',
    weekday: 'short',
    hour: 'numeric',
    hour12: false,
  }).formatToParts(data)

  return {
    diaSemana: partes.find((parte) => parte.type === 'weekday')?.value ?? '',
    hora: Number(partes.find((parte) => parte.type === 'hour')?.value ?? 0),
  }
}

export function mercadoAberto(data = new Date()) {
  const { diaSemana, hora } = partesDataSaoPaulo(data)
  const fimDeSemana = diaSemana.startsWith('sáb') || diaSemana.startsWith('dom')

  return !fimDeSemana && hora >= HORA_ABERTURA_PREGAO && hora < HORA_FECHAMENTO_PREGAO
}
