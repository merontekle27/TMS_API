# TMS API Versioning & Deprecation Policy

## 1. Purpose & Principles
The Training Management System (TMS) API serves diverse clients, including web dashboards, third-party integrations, and field tablets deployed in rural training centers operating on periodic maintenance cycles. To balance rapid feature innovation with contract stability, all TMS API changes must adhere strictly to this policy.

## 2. Classification of Changes

### Breaking Changes (Require New Major Version, e.g., V1 -> V2)
Any modification that invalidates existing client assumptions or parsers constitutes a breaking change:
- Removing or renaming any request or response field.
- Changing data types or structure (e.g., converting a scalar to an array, flattening or nesting existing JSON envelopes).
- Changing HTTP response status codes for existing scenarios.
- Tightening validation rules (e.g., making optional fields required, restricting regex formats, or lowering string max lengths).
- Changing default behaviors, such as default sort order, filtering semantics, or pagination rules.

### Additive / Non-Breaking Changes (Permitted Within Same Version)
Non-breaking changes maintain backwards compatibility with existing consumers:
- Adding a new optional request field or query parameter.
- Adding a new field to a response body.
- Introducing a completely new endpoint or HTTP method on an existing resource.
- Relaxing validation constraints.

## 3. Sunset & Deprecation Window
When a new major API version is released, earlier versions enter an official **Deprecation & Sunset Window**:
- **Minimum Support Window**: Any deprecated version will remain fully operational for a **minimum of 6 months** post-release of its successor. This guarantees that rural centers and quarterly deployment cycles can plan and test upgrades without disruption.
- **Strict Sunset Date**: Following the sunset date, deprecated endpoints return `410 Gone` or are deactivated.

## 4. Communication Strategy & Protocol Signaling
Deprecation is communicated directly via protocol-level HTTP headers and developer outreach:
1. **HTTP Deprecation Headers**: From the day a new version ships, all deprecated responses include:
   - `Deprecation: true` (Signals deprecation status per IETF draft)
   - `Sunset: <RFC 7231 Date>` (Specifies the decommission timestamp per RFC 8594)
   - `Link: <...>; rel="successor-version"` (Directs clients to the successor resource per RFC 5988)
2. **Developer Outreach**:
   - Immediate release notes in `CHANGELOG.md`.
   - Direct notification sent to all registered API key holders and integration leads.
   - Calendar invite and automated reminder intervals (90, 60, and 30 days before sunset).

## 5. Version Progression & Skipping Versions
- Clients are not forced to migrate through every intermediate version sequentially. Direct upgrades (e.g., `V1 -> V3`) are supported and documented with consolidated migration guides.

## 6. Routing Mechanism & Escape Hatch
- **Primary Mechanism**: URL segment versioning (e.g., `/api/v1/courses` and `/api/v2/courses`). URL paths provide clarity, transparency in logs, and deterministic routing.
- **Escape Hatch**: For partners with caching proxies or rigid URL structures, header-based resolution is supported via the `X-Api-Version` header (e.g., `X-Api-Version: 2.0` targeting `/api/courses`).
